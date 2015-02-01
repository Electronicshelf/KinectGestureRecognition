using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.IO.Ports;
using System.IO;

namespace GestureRecognizer
{
   public class GestureRecognitionEngine
    {


        public GestureRecognitionEngine()
    {
    }
        string[] servoId = new string[6] { "INIT", "BASE", "SHLD", "ELBW", "WRST", "GRPR" };
        int[] raMove = new int[6] { 7, 170, 165, 200, 240, 160 };
        SerialPort port = new SerialPort("COM15", 19200, Parity.None, 8, StopBits.One);
        public event EventHandler<GestureEventArgs>GestureRecognized;
       // public event EventHandler<GestureEventArgs>GestureNotRecognized;
        public GestureType  GestureType { get; set; }
        public Skeleton skeleton;

       private float GetJointDistance(Joint firstJoint, Joint secondJoint)
        {
            float distanceX = firstJoint.Position.X - secondJoint.Position.X;
            float distanceY = firstJoint.Position.Y - secondJoint.Position.Y;
            float distanceZ = firstJoint.Position.Z - secondJoint.Position.Z;
            return (float)Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2) + Math.Pow(distanceZ, 2));
        }


        public void StartRecognise()
        {
            switch (GestureType)
            {
                case GestureType.HandClapping:
                    this.MatchClappingGesture(this.skeleton);
                    break;
               default:
                    break;
            }
           
        }


        public void setArm(int[] raMove, SerialPort port, string[] servoId)
        {
            string Command;
            string servo;
            string[] newAngles = new string[6];
            byte[] PositionByte = new byte[6];
            byte b;
            int a = 10;

            //sets all the servos of the arm 
            //goes through each motor Value and  converts it to hex
            string c = a.ToString("X");
            b = byte.Parse(c, System.Globalization.NumberStyles.HexNumber);
            port = new SerialPort("COM15", 19200, Parity.None, 8, StopBits.One);

            try
            {

                port.Open();
                for (int i = 0; i < 6; i++)
                {
                    newAngles[i] = raMove[i].ToString("X");
                    //Console.WriteLine(newAngles[i]);
                    PositionByte[i] = byte.Parse(newAngles[i], System.Globalization.NumberStyles.HexNumber);
                }
                //Send Data to Serial Port
                int delay = 150;
                for (int j = 0; j < delay; j++)
                {
                    int g = 6;
                    for (int i = 0; i < g; i++)
                    {
                        servo = servoId[i];
                        Command = string.Format(" {0} {1} ", servo, raMove[i]);
                        //Send Control Serial Command
                        port.Write(new byte[] { PositionByte[i] }, 0, 1);
                        //Console.WriteLine(" The value sent to >>> {0} ", servo + " >>>> " + PositionByte[i]);
                    }
                }

            }

            finally
            {
                // Console.WriteLine("---# Command  Stream Sent ---#");
                port.Close();
            }
        }
        

        float previousDistance = 0.0f;
        
        private void MatchClappingGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return;
            }

            if (skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked && skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
            {
                float currentDistance = GetJointDistance(skeleton.Joints[JointType.HandRight],skeleton.Joints[JointType.HandLeft]);
            {
            
                if (currentDistance < 0.1f && previousDistance > 0.1f)
            {
           
                    if (this.GestureRecognized != null)
            {
           
                        this.GestureRecognized(this, new GestureEventArgs(RecognitionResult.Success));
            }

            }
            previousDistance = currentDistance;
            
        }
            //previousDistance -= 0.2f;
            }

        }
   
   
    

    }

}

