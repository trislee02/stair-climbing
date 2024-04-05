using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class SkeletalTrackingProvider : BackgroundDataProvider
{
    bool readFirstFrame = false;
    TimeSpan initialTimestamp;

    public SkeletalTrackingProvider(int id) : base(id)
    {
        Debug.Log("in the skeleton provider constructor");
    }

    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter { get; set; } = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

    public Stream RawDataLoggingFile = null;

    protected override void RunBackgroundThreadAsync(int id, CancellationToken token)
    {
        try
        {
            UnityEngine.Debug.Log("Starting body tracker background thread.");

            // Buffer allocations.
            BackgroundData currentFrameData = new BackgroundData();
            Body currentBody = new Body(BackgroundData.MAX_BODY_JOINT_SIZE);
            // Open device.
            using (Device device = Device.Open(id))
            {
                device.StartCameras(new DeviceConfiguration()
                {
                    CameraFPS = FPS.FPS30,
                    ColorResolution = ColorResolution.Off,
                    DepthMode = DepthMode.NFOV_Unbinned,
                    WiredSyncMode = WiredSyncMode.Standalone,
                });

                UnityEngine.Debug.Log("Open K4A device successful. id " + id + "sn:" + device.SerialNum);

                var deviceCalibration = device.GetCalibration();
                
                using (Tracker tracker = Tracker.Create(deviceCalibration, new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Cuda, SensorOrientation = SensorOrientation.Default }))
                {
                    UnityEngine.Debug.Log("Body tracker created.");
                    while (!token.IsCancellationRequested)
                    {
                        using (Capture sensorCapture = device.GetCapture())
                        {
                            // Queue latest frame from the sensor.
                            tracker.EnqueueCapture(sensorCapture);
                        }

                        // Try getting latest tracker frame.
                        using (Frame frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false))
                        {
                            if (frame == null)
                            {
                                UnityEngine.Debug.Log("Pop result from tracker timeout!");
                            }
                            else
                            {
                                IsRunning = true;
                                if (frame.NumberOfBodies > 0)
                                {
                                    // Get number of bodies in the current frame.
                                    currentFrameData.CouldHasData = true;// frame.NumberOfBodies;
                                    // Copy bodies.
                                    currentBody.CopyFromBodyTrackingSdk(frame.GetBody(0), deviceCalibration);
                                    currentFrameData.Left = currentBody.JointPositions3D[(int)BackgroundData.LEFT_JOINT_ID].Y;
                                    currentFrameData.Right = currentBody.JointPositions3D[(int)BackgroundData.RIGHT_JOINT_ID].Y;

                                    //Debug.Log("Euler angle: " + currentBody.JointRotations[(int)JointId.KneeLeft]);

                                    Vector3 hipLeftPosition = new Vector3(currentBody.JointPositions3D[(int)JointId.HipLeft].X,
                                                                            currentBody.JointPositions3D[(int)JointId.HipLeft].Y,
                                                                            currentBody.JointPositions3D[(int)JointId.HipLeft].Z);

                                    Vector3 hipRightPosition = new Vector3(currentBody.JointPositions3D[(int)JointId.HipRight].X,
                                                                            currentBody.JointPositions3D[(int)JointId.HipRight].Y,
                                                                            currentBody.JointPositions3D[(int)JointId.HipRight].Z);

                                    Vector3 kneeLeftPosition = new Vector3(currentBody.JointPositions3D[(int)JointId.KneeLeft].X,
                                                                            currentBody.JointPositions3D[(int)JointId.KneeLeft].Y,
                                                                            currentBody.JointPositions3D[(int)JointId.KneeLeft].Z);

                                    Vector3 kneeRightPosition = new Vector3(currentBody.JointPositions3D[(int)JointId.KneeRight].X,
                                                                            currentBody.JointPositions3D[(int)JointId.KneeRight].Y,
                                                                            currentBody.JointPositions3D[(int)JointId.KneeRight].Z);

                                    Vector3 ankleLeftPosition = new Vector3(currentBody.JointPositions3D[(int)JointId.AnkleLeft].X,
                                                                            currentBody.JointPositions3D[(int)JointId.AnkleLeft].Y,
                                                                            currentBody.JointPositions3D[(int)JointId.AnkleLeft].Z);

                                    Vector3 ankleRightPosition = new Vector3(currentBody.JointPositions3D[(int)JointId.AnkleRight].X,
                                                                            currentBody.JointPositions3D[(int)JointId.AnkleRight].Y,
                                                                            currentBody.JointPositions3D[(int)JointId.AnkleRight].Z);

                                    // calculate bone length
                                    float hipKneeBoneLeft = Vector3.Distance(hipLeftPosition, kneeLeftPosition);
                                    float hipKneeBoneRight = Vector3.Distance(hipRightPosition, kneeRightPosition);
                                    float kneeAnkleBoneLeft = Vector3.Distance(kneeLeftPosition, ankleLeftPosition);
                                    float kneeAnkleBoneRight = Vector3.Distance(kneeRightPosition, ankleRightPosition);

                                    Vector3 hipKneeLeft = hipLeftPosition - kneeLeftPosition;
                                    Vector3 hipKneeRight = hipRightPosition - kneeRightPosition;
                                    Vector3 kneeAnkleLeft = ankleLeftPosition - kneeLeftPosition;
                                    Vector3 kneeAnkleRight = ankleRightPosition - kneeRightPosition;

                                    float angleLeft = Vector3.Angle(hipKneeLeft, kneeAnkleLeft);
                                    float angleRight = Vector3.Angle(hipKneeRight, kneeAnkleRight);

                                    // cal
                                    float opAngleLeft = 180f - angleLeft;
                                    float opAngleRight = 180f - angleRight;
                                    float tmpSegmentLeft = Mathf.Cos(opAngleLeft / 180f * Mathf.PI) * hipKneeBoneLeft;
                                    float tmpSegmentRight = Mathf.Cos(opAngleRight / 180f * Mathf.PI) * hipKneeBoneRight;
                                    float heightLeft = hipKneeBoneLeft - tmpSegmentLeft;
                                    float heightRight = hipKneeBoneRight - tmpSegmentRight;

                                    Debug.Log("Angle left: " + angleLeft + ", Angle right: " + angleRight);
                                    Debug.Log("Computed height left: " + heightLeft + ", Height right: " + heightRight);

                                    //currentFrameData.Left = frame.GetBody(0).Skeleton.GetJoint(BackgroundData.LEFT_JOINT_ID).Position.Y;
                                    //currentFrameData.Right = frame.GetBody(0).Skeleton.GetJoint(BackgroundData.RIGHT_JOINT_ID).Position.Y;
                                }
                                else
                                {
                                    currentFrameData.CouldHasData = false;
                                }

                                // Store depth image.
                                //Capture bodyFrameCapture = frame.Capture;
                                //Image depthImage = bodyFrameCapture.Depth;
                                //if (!readFirstFrame)
                                //{
                                //    readFirstFrame = true;
                                //    initialTimestamp = depthImage.DeviceTimestamp;
                                //}
                                //currentFrameData.TimestampInMs = (float)(depthImage.DeviceTimestamp - initialTimestamp).TotalMilliseconds;
                                //currentFrameData.DepthImageWidth = depthImage.WidthPixels;
                                //currentFrameData.DepthImageHeight = depthImage.HeightPixels;

                                // Read image data from the SDK.
                                //var depthFrame = MemoryMarshal.Cast<byte, ushort>(depthImage.Memory.Span);

                                // Repack data and store image data.
                                //int byteCounter = 0;
                                //currentFrameData.DepthImageSize = currentFrameData.DepthImageWidth * currentFrameData.DepthImageHeight * 3;

                                //for (int it = currentFrameData.DepthImageWidth * currentFrameData.DepthImageHeight - 1; it > 0; it--)
                                //{
                                //    byte b = (byte)(depthFrame[it] / (ConfigLoader.Instance.Configs.SkeletalTracking.MaximumDisplayedDepthInMillimeters) * 255);
                                //    currentFrameData.DepthImage[byteCounter++] = b;
                                //    currentFrameData.DepthImage[byteCounter++] = b;
                                //    currentFrameData.DepthImage[byteCounter++] = b;
                                //}

                                //if (RawDataLoggingFile != null && RawDataLoggingFile.CanWrite)
                                //{
                                //    binaryFormatter.Serialize(RawDataLoggingFile, currentFrameData);
                                //}

                                // Update data variable that is being read in the UI thread.
                                SetCurrentFrameData(ref currentFrameData);
                            }

                        }
                    }
                    Debug.Log("dispose of tracker now!!!!!");
                    tracker.Dispose();
                }
                device.Dispose();
            }
            if (RawDataLoggingFile != null)
            {
                RawDataLoggingFile.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log($"catching exception for background thread {e.Message}");
            token.ThrowIfCancellationRequested();
        }
    }
}