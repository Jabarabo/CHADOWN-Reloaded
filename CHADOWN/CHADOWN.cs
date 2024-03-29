﻿using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace CHADOWN
{
    public static class CHADMethods //Class containing all the functional methods for CHADOWN.
    {
        public static void AVComboMethod(string OutputDir) //Main part of the A/V Combo module.
        {
            Console.Clear();
            Console.WriteLine("Select the video file you wish to use.");
            OpenFileDialog Videofileselect = new OpenFileDialog() //The following code opens a file explorer dialog so the video file can be chosen. More formats will be added when I clean this up a bit.
            {
                Title = "Select video file",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "MP4 Files (*.mp4)|*.mp4*|All Files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (Videofileselect.ShowDialog() == DialogResult.OK)
            {

            }
            Console.Clear();
            Console.WriteLine("Select the video file you wish to use.");
            OpenFileDialog Audiofileselect = new OpenFileDialog() //The following code opens a file explorer dialog so the audio file can be chosen. More formats will be added when I clean this up a bit.
            {
                Title = "Select audio file",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "MP3 Files (*.mp3)|*.mp3*|WAV Files (*.wav)|*.wav*|MP4 Files (*.mp4)|*.mp4*|All Files (*.*)|*.*",
                FilterIndex = 4,
                RestoreDirectory = true
            };
            if (Audiofileselect.ShowDialog() == DialogResult.OK)
            {

            }
            Console.Clear();
            Console.WriteLine("What would you like the file to be named? Don't include the file extension in the name.");
            string OutputName = Console.ReadLine(); //Video file output is named.
            string AVCInputs = $"/k ffmpeg -i \"{Videofileselect.FileName}\" -i \"{Audiofileselect.FileName}\" -c copy \"{OutputDir}\\{OutputName}.mp4\"";
            Process p = Process.Start("CMD.exe", AVCInputs);
            p.WaitForExit();
        }
        public static void YTDLmethod(string OutputDir) //Main part of the YTDL module.
        {
            string outputFormat = ""; //What yt-dl is told to output as.
            string fileExtension = ""; //The actual extension used for the file.
            string YTDLtype = ""; //Decides whether it will become an audio or A/V file.
            string fileName = ""; //Name of the file without the extension.
            string videoURL = ""; //URL of the video.
            
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Would you like the file to be Audio/Video or just audio?\n");
                Console.WriteLine("(a) Audio/Video");
                Console.WriteLine("(b) Just Audio");
                YTDLtype = Console.ReadLine();
                Console.Clear();
                if (YTDLtype == "b")
                {
                    outputFormat = "--extract-audio --audio-format mp3 --audio-quality 0";
                    fileExtension = ".mp3";
                    break;
                }
                else if (YTDLtype == "a")
                {
                    outputFormat = "--format mp4";
                    fileExtension = ".mp4";
                    break;
                }
                else
                {
                    continue;
                }
            }
            Console.Clear();
            
            Console.WriteLine("What would you like to name the video (Without file extension)?");
            fileName = Console.ReadLine();
            Console.Clear();
            
            Console.WriteLine("Paste the URL of the Youtube video.");
            videoURL = Console.ReadLine();
            Console.Clear();
            
            string strCmdText = $"/K yt-dlp {videoURL} --output \"{OutputDir}\\{fileName + fileExtension}\" {outputFormat}";
            Process p = Process.Start("CMD.exe", strCmdText);
            p.WaitForExit();
        }
        public static void FMPGmethod(string OutputDir) //Main part of the ffmpeg module.
        {
            Console.WriteLine("Please paste the link for the playlist (usually a .m3u8) file");
            string FMPGlink = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("What would you like to name the video (Don't include the filetype in the name)?");
            string FMPGvideoname = Console.ReadLine();
            Console.Clear();
            string strCmdText = $"/C ffmpeg -protocol_whitelist file,http,https,tcp,tls,crypto -i {FMPGlink} \"{OutputDir}\\{FMPGvideoname}.mp4\"";
            Process p = Process.Start("CMD.exe", strCmdText);
            p.WaitForExit();
        }
        public static void TCol(string RText, string WText)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(RText);
            Console.ResetColor();
            Console.Write(WText);
        }
    }
   
    public class ConfigIO//Handles creating, writing to, and rewriting the config.
    {
        string ConfigPath = Directory.GetCurrentDirectory() + @"\" + "config.txt";
        public void CreateConfig()//Creates the config if one is not found.
        {
            if (!File.Exists(ConfigPath)) //If the config file doesn't exist, this creates it and assigns the user's Videos folder as the default.
            {
                using (FileStream ConCreate = File.Create(ConfigPath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
                    ConCreate.Write(info, 0, info.Length);
                }
            }
        }
        public string ReadConfig()//Reads the output directory from the config file and copies it into memory.
        {
            string Outpath = System.IO.File.ReadAllText(ConfigPath);
            return Outpath;
        }
        public void RewriteConfig()//Lets the user select a new output directory, deletes the old config, and replaces it with the new one.
        {
            string Outpath = System.IO.File.ReadAllText(ConfigPath);
            Console.Clear();
            Console.WriteLine("Select the new output directory.");
            FolderBrowserDialog NewOutputDir = new FolderBrowserDialog(); //Creates the object for finding the folder.
            if (NewOutputDir.ShowDialog() == System.Windows.Forms.DialogResult.OK) //Opens dialog and user selects new output folder.
            {
                Outpath = NewOutputDir.SelectedPath;
            };
            Console.Clear();
            if (File.Exists(ConfigPath)) //Finds the existing config file and deletes it.
            {
                File.Delete(ConfigPath);
            }
            using (FileStream ConCreate = File.Create(ConfigPath)) //Creates and writes to the new config file.
            {
                byte[] info = new UTF8Encoding(true).GetBytes(Outpath);
                ConCreate.Write(info, 0, info.Length);
            }
        }
    }
    public static class ElectricQueuegaloo //Contains Queue-compatable versions of the CHADOWN functions (VERY WORK IN PROGRESS).
    {
        public static void FMPGQueue(string OutputDir) //FMPG queue function
        {
            Console.Clear();
            
            int i = 0;
            bool StillQueueing = true;
            Queue<string> QVideos = new Queue<string>();
            
            while(StillQueueing == true)
            {
                Console.Clear();
                Console.WriteLine("Please enter the URL of your queue object. Enter 0 to finalize your queue.");
                Console.WriteLine("Objects in queue:" + i);
                string VidURL = Console.ReadLine();
                if (VidURL == "0")
                {
                    StillQueueing = false;
                }else{
                    QVideos.Enqueue(VidURL);
                }
                Console.Clear();
                i++;
            }
            
            Console.Clear();
            i = 0;
            
            foreach (string QVideo in QVideos)
            {
                i++;
                string FMPGlink = QVideo;
                string FMPGvideoname = "QueueObject " + i;
                string strCmdText = $"/C ffmpeg -protocol_whitelist file,http,https,tcp,tls,crypto -i {FMPGlink} \"{OutputDir}\\{FMPGvideoname}.mp4\"";
                Process p = Process.Start("CMD.exe", strCmdText);
            }
        }
        public static void YTDLQueueA(string OutputDir)
        {
            Console.Clear();

            int i = 0;
            bool StillQueueing = true;
            Queue<string> QVideos = new Queue<string>();

            while (StillQueueing == true)
            {
                Console.Clear();
                Console.WriteLine("Please enter the URL of your queue object. Enter 0 to finalize your queue.");
                Console.WriteLine("Objects in queue:" + i);
                string VidURL = Console.ReadLine();
                if (VidURL == "0")
                {
                    StillQueueing = false;
                }
                else
                {
                    QVideos.Enqueue(VidURL);
                }
                i++;
            }
            Console.Clear();
            i = 0;

            foreach (string QVideo in QVideos)
            {

            }
        }
    }
    class CHADOWN
    {
        [STAThread]
        static void Main(string[] args)
        {
            ConfigIO config = new ConfigIO();
            config.CreateConfig();
            while (true)
            {
                config.ReadConfig();
                string outpath = config.ReadConfig();
                Console.WriteLine();
                Console.Write("Welcome to Jabs' ");
                CHADMethods.TCol("Ch", "eap ");
                CHADMethods.TCol("A", "ss ");
                CHADMethods.TCol("Down", "loader!\n");
                Console.WriteLine("Please select which type of video you wish to *procure*");
                Console.WriteLine("---------------");
                Console.WriteLine("A. Youtube Videos");
                Console.WriteLine("B. BLOB/M3U8 Streams");
                Console.WriteLine("C. Merge audio and video files.");
                Console.WriteLine("D. BLOB/M3U8 Streams 2: Electric Queuegaloo.\n\n\n");
                Console.Write("Press 0 to change where videos are output to.\n");
                Console.WriteLine($"Current output dir is {outpath}");
                string ConInputType = Console.ReadLine(); //User selects which one they want.
                Console.Clear();
                switch (ConInputType) //Allows the user to select which module they would like to use.
                {
                    case "A": CHADMethods.YTDLmethod(outpath); break;
                    case "B": CHADMethods.FMPGmethod(outpath); break;
                    case "C": CHADMethods.AVComboMethod(outpath); break;
                    case "D": ElectricQueuegaloo.FMPGQueue(outpath); break;
                    case "E": //Nothing yet. Will be Youtube-dl 2 Electric Queuegaloo
                    case "0": config.RewriteConfig(); break;
                    default: continue;
                }
                Console.Clear();
                Console.WriteLine("Continue? (y/n)");
                if (Console.ReadLine() == "n") //The user chooses if they need to do another video.
                {
                    return; //Kill script by returning from main.
                }
                Console.Clear();
            }
        }
    }
}