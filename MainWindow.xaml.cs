using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.IO;
using System.IO.Ports;

using GestureRecognizer;


namespace ClappingHands
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
   
    public partial class MainWindow : Window
    {


        string[] servoId = new string[6] { "INIT", "BASE", "SHLD", "ELBW", "WRST", "GRPR" };
        int[] raMove = new int[6] { 7, 170, 165, 200, 240, 160 };
        SerialPort port = new SerialPort("COM15", 19200, Parity.None, 8, StopBits.One);

        private KinectSensor kinect;
        private const int skeletonCount = 6;
        private Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        GestureRecognitionEngine recognitionEngine;
        //ArmController armSet;
       
        public MainWindow()
        {
            
            InitializeComponent();
        }
        
         private void Window_Loaded(object sender, RoutedEventArgs e)
         {
            ;
            try
            {
                kinect = KinectSensor.KinectSensors[0];
                kinect.Start();
               
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Could not find Kinect Camera: " + ex.Message);
            }
            
            kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            kinect.ColorStream.Enable(ColorImageFormat.RgbResolution1280x960Fps12);
            kinect.SkeletonStream.Enable(new TransformSmoothParameters()
            {
                Correction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.05f,
                Prediction = 0.5f,
                Smoothing = 0.5f
            });
            kinect.AllFramesReady += Kinect_SkeletonAllFramesReady;
            
           
            recognitionEngine = new GestureRecognitionEngine();
            //recognitionEngine.GestureType = GestureType.HandClapping;
            recognitionEngine.GestureRecognized += new EventHandler<GestureEventArgs>(recognitionEngine_GestureRecognized);
           

        }
         void recognitionEngine_GestureRecognized(object sender, GestureEventArgs e)
         {
             //armSet.setArm(raMove, port, servoId);
            // recognitionEngine.setArm(raMove,port,servoId);
             MessageBox.Show("CLAP!!!");
         }

        private void Window_Closed(object sender, EventArgs e)
        {
            kinect.Stop();
        }
        private void Kinect_SkeletonAllFramesReady(object source, AllFramesReadyEventArgs e)
        {
            ColorImageFrame colorImageFrame = null;
            DepthImageFrame depthImageFrame = null;
            SkeletonFrame skeletonFrame = null;
            try
            {
                colorImageFrame = e.OpenColorImageFrame();
                depthImageFrame = e.OpenDepthImageFrame();
                skeletonFrame   = e.OpenSkeletonFrame();
               // image.Source = depthImageFrame.ToBitmapSource();
                image.Source = colorImageFrame.ToBitmapSource();
                if (skeletonFrame != null)
                {
                    skeletonFrame.CopySkeletonDataTo(allSkeletons);
                }


                foreach(Skeleton firstskeleton in allSkeletons)
                {
                
                if (firstskeleton.TrackingState ==  SkeletonTrackingState.Tracked)
                {
                    
                    if (firstskeleton == null)
                    {
                        return;
                    }
                    
                    recognitionEngine.skeleton = firstskeleton;
                    recognitionEngine.StartRecognise();
                }
                }
                    
            }
                
            finally
            {
                if (colorImageFrame != null)
                    colorImageFrame.Dispose();
                if (depthImageFrame != null)
                    depthImageFrame.Dispose();
                if (skeletonFrame != null)
                    skeletonFrame.Dispose();
                
            }
        }
        }

          
       
        }
        
        





    

