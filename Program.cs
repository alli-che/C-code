using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using System.Threading;

namespace WindowsBuild
{
    class Program
    {
        public static string cwd;
        public static string rawBrain;
        public static string rawBody;
        public static string upSampledBrain;
        public static string upSampledBody;
        public static string parentDir;

        /*
         * need to ask for the path to python
         
         exceptions to check for:
            file not found
            directory not found
            system argument acception

             
             
             */



        public static string PyCall(string rawPath, string upsampledPath, string pyFile, string exeForPy)
        {

            string path = Directory.GetCurrentDirectory();
            Console.Write(path + "\n\n\n");
            Process process = new Process();
            process.StartInfo.FileName = @exeForPy;

            process.StartInfo.Arguments = rawPath + pyFile + " " + upsampledPath + " " + rawPath;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            try
            {
                process.Start();
            }

            catch (System.ComponentModel.Win32Exception winE){

                Console.Write("This was not a valid path " + winE.Message);
                Console.Write("\nTo exit press E. To try again press A:   ");
                string str = Console.ReadLine();
                if (string.Equals(str, "E"))
                {
                    return "";
                }
                else if (string.Equals(str, "A"))
                {
                    return "noVal";
                }
                else {
                    return "noVal";

                }

                
            }
            
            StreamReader sReader = process.StandardOutput;
            string output = sReader.ReadToEnd();

            Console.Write(output);
            process.WaitForExit();
            return "success";
        }



        public static void UnityCall()
        {

            string path = Directory.GetCurrentDirectory();
            Console.Write(path + "\n");
            Console.Write(path + "\n\n\n");
            Process process = new Process();
            process.StartInfo.FileName = path + @"\BBFApp.exe";

           // process.StartInfo.Arguments = rawPath + pyFile + " " + upsampledPath + " " + rawPath;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            StreamReader sReader = process.StandardOutput;
            string output = sReader.ReadToEnd();

            Console.Write(output);
            process.WaitForExit();
        }












        public static void Options()
        {
            Console.Write("Finding File locations.... \n");
            FileLocations();

            string response = "";
            while (true)
            {
                response = Questions();

                if(string.Equals(response, "0"))
                {
                    Console.Write("\n It is important that you do not remove any files from the this folder. To use this app" +
                        "you must give everything in full paths like so " 
                        + cwd + " \n\n\nThis app requires many python libraries. It would be your best interest to download Anaconda3." +
                        " Once you have done this you should add Anaconda3 to your user path. This can be done by typing 'where python' in" +
                        " the command prompt. Then edit your environment variables to change the path. Once this is done open a new command prompt and " +
                        "type 'where python' again. Remember this path when asked for the path to your python executable. It should look something like this " +
                        @"C:\Users\You\Anaconda3\python.exe." + "\n\n\n\n");

                }
                else if (string.Equals(response, "1"))
                {
                    Console.Write("Confirm with Y to copy files. Confirm with N to go back to Main Menu. ");
                    string confirm =  Console.ReadLine();

                    if (string.Equals(confirm, "Y"))
                    {

                        bool goodPath = false;
                        string src = "";
                        //has to be actual full path 
                        while (!goodPath)
                        {

                            string source = GetRawPath("Brain");

                            if (!string.Equals(source, "none"))
                            {
                                goodPath = true;
                                src = source;
                            }



                        }
                        CopyFiles(src, rawBrain);

                    }
                    else {
                       
                    }
                   


                }
                else if (string.Equals(response, "2"))
                {

                    Console.Write("Confirm with Y to copy files. Confirm with N to go back to Main Menu. ");
                    string confirm = Console.ReadLine();

                    if (string.Equals(confirm, "Y"))
                    {
                        bool goodPath = false;
                        string src = "";
                        //has to be actual full path 
                        while (!goodPath)
                        {

                            string source = GetRawPath("Body");

                            if (!string.Equals(source, "none"))
                            {
                                goodPath = true;
                                src = source;
                            }



                        }
                        CopyFiles(src, rawBody);
                    }
                    else {

                    }

                }
                else if (string.Equals(response, "3"))
                {

                    Console.Write("Confirm with Y to Up Sample the files. Confirm with N to go back to Main Menu.  ");
                    string confirm = Console.ReadLine();

                    if (string.Equals(confirm, "Y"))
                    {
                        bool upSample = true;
                        while (upSample)
                        {
                            Console.Write("Enter the path to Anaconda3 python 3.6 exe. Do not use any spaces. ");
                            string pythonPath = Console.ReadLine();
                            Console.Write("\nUpsampling the Brain Data .... \n");
                            string rsp = PyCall(rawBrain, upSampledBrain, @"\FNIRS.py", pythonPath);
                            if(string.Equals(rsp, "noVal"))
                            {

                            }
                            else if(string.Equals(rsp, "success"))
                            {
                                upSample = false;
                            }
                            else
                            {
                                upSample = false;
                            }
                        }
                        
                    }
                    else
                    {

                    }


                }
                else if (string.Equals(response, "4"))
                {

                    Console.Write("Confirm with Y to Up Sample the files. Confirm with N to go back to Main Menu. ");
                    string confirm = Console.ReadLine();

                    if (string.Equals(confirm, "Y"))
                    {
                       
                        bool upSample = true;
                        while (upSample)
                        {
                            Console.Write("Enter the path to Anaconda3 python 3.6 exe. Do not use any spaces. ");
                            string pythonPath = Console.ReadLine();
                            Console.Write("\nUpsampling the Body Data .... \n");
                            string rsp = PyCall(rawBody, upSampledBody, @"\MOCAP.py", pythonPath);
                            if (string.Equals(rsp, "noVal"))
                            {

                            }
                            else if (string.Equals(rsp, "success"))
                            {
                                upSample = false;
                            }
                            else
                            {
                                upSample = false;
                            }
                        }
                    }
                    else {

                    }
                }
                else if (string.Equals(response, "5"))
                {
                    Console.Write("Making a call to unity... \n");
                    UnityCall();

                }
                else if (string.Equals(response, "6"))
                {
                 

                }
                else if (string.Equals(response, "7"))
                {
                    Console.Write("Select which directories you would like empty \n" +
                        "1. Raw Brain Data \n" +
                        "2. Raw Body Data \n" +
                        "3. Up Sampled Brain Data \n" +
                        "4. Up Sampled Body Data \n" +
                        "5. Back to Main Menu   \n " +
                        "Enter your answer : " );

                    string myRsp = Console.ReadLine();

                    bool rspBool = true;

                    while (rspBool)
                    {

                        if (string.Equals(myRsp, "1") || string.Equals(myRsp, "2"))
                        {
                            string delDirectory = getDir(myRsp);

                            txtfileDeletion(delDirectory);

                            rspBool = false;
                        }
                        else if (string.Equals(myRsp, "3") || string.Equals(myRsp, "4"))
                        {
                            string delDirectory = getDir(myRsp);

                            csvfileDeletion(delDirectory);

                            rspBool = false;

                        }
                        else if (string.Equals(myRsp, "5"))
                        {
                            rspBool = false;
                        }
                        else
                        {

                            Console.Write("Select which directories you would like to empty: \n" +
                            "1. Raw Brain Data \n" +
                            "2. Raw Body Data \n" +
                            "3. Up Sampled Brain Data \n" +
                            "4. Up Sampled Body Data \n" +
                            "5. Back to main menu \n" +
                            "Enter your answer : ");

                            myRsp = Console.ReadLine();
                        }


                    }   
                }
                else if (string.Equals(response, "8"))
                {
                    System.Environment.Exit(1);
                }
                else
                {
                    Console.Write("Sorry, that was not an option. Please answer the questions again \n \n");
                }
            }

        }

        public static string Questions()
        {

            Console.Write("\nWelcome to the Brain and Body Fusion Application Main Menu \n Please review the following options: \n" +
                         "0. Instructions \n" +
                         "1. Copy Raw Brain Data \n" +
                         "2. Copy Raw Mocap Data \n" +
                         "3. Upsample Brain Data \n" +
                         "4. Upsample Mocap Data \n" +
                         "5. Start Unity \n" +
                         "6. Bring up Main Menu \n" +
                         "7. Delete all files \n" +
                         "8. Quit \n");

            Console.Write("Enter numbers 0-8 :  ");

            string number = Console.ReadLine();
            return number;

        }

        public static string GetRawPath(string choice) {

            Console.Write("\nOption: Copy raw " + choice + " Data \n");
            Console.Write("Enter the full path to the raw data  :   ");
            string source = Console.ReadLine();
            try
            {
               // Console.Write("in the try");
                DirectoryInfo dirClear = new DirectoryInfo(source);
                FileInfo[] files = dirClear.GetFiles();

            }

            catch (System.ArgumentException noArg)
            {
                Console.Write("This directory does not exits. Please type in a directory that exits. " + noArg.Message + "\n");
                //Console.Write("in the catch\n");
                return "none";
            }

            catch (DirectoryNotFoundException noDir)
            {
                Console.Write("This directory does not exits. Please type in a directory that exits. " + noDir.Message + "\n");
              //  Console.Write("in the catch\n");
                return "none";
            }
            return source;

        }


        public static string getDir(string rsp)
        {
            switch (rsp)
            {
                case "1": return rawBrain;
                case "2": return rawBody;
                case "3": return upSampledBrain;
                case "4": return upSampledBody;
                default: return "none";
            }

        }


        public static void CopyFiles(string source, string target)
        {

            DirectoryInfo dirClear = new DirectoryInfo(source);
            FileInfo[] files = dirClear.GetFiles();

            Console.Write("Copying files... \n");
           
            foreach (FileInfo file in files)
            {

                string tpath = Path.Combine(target, file.Name);
                try {
                    file.CopyTo(tpath, false);
                }
                catch (IOException noIO)
                {

                    Console.Write(noIO.Message + "\n" + "Loading Main Menu options.... \n" );
                    //                    Console.Write("Successfully copied all files \n");
                    return;

                } 
            }

            Console.Write("Successfully copied all files... \n");
        }


        public static void txtfileDeletion(string path) {

            string[] delFiles = Directory.GetFiles(path, "*.txt");

            if (delFiles.Length == 0)
            {
                Console.Write("\nThere are no files to be deleted. \n");
            }
            else {

                for (int i = 0; i < delFiles.Length; i++)
                {
                    Console.Write(delFiles[i] + "\n");
                }

                Console.Write("Are you sure you want to delete ALL of these files? \nEnter Y for yes N for main menu \n");
                string str = Console.ReadLine();

                if (string.Equals(str, "Y"))
                {
                    foreach (string txtDel in delFiles)
                    {
                        File.Delete(txtDel);
                    }

                    Console.Write("All files deleted... \n");
                }
                else
                {

                }

            }
        }


        public static void csvfileDeletion(string path)
        {

            string[] delFiles = Directory.GetFiles(path, "*.csv");

            if (delFiles.Length == 0)
            {
                Console.Write("\nThere are no files to be deleted. \n");
            }
            else
            {

                for (int i = 0; i < delFiles.Length; i++)
                {
                    Console.Write(delFiles[i] + "\n");
                }

                Console.Write("Are you sure you want to delete ALL of these files? \nEnter Y for yes N for main menu \n");
                string str = Console.ReadLine();

                if (string.Equals(str, "Y"))
                {
                    foreach (string csvDel in delFiles)
                    {
                        File.Delete(csvDel);
                    }

                    Console.Write("All files deleted... \n");
                }
                else
                {

                }

            }
        }


        public static void FileLocations()
        {
            cwd = Directory.GetCurrentDirectory();
            Console.Write(cwd + "\n");

            DirectoryInfo parDir = Directory.GetParent(cwd);
            parentDir = parDir.FullName;

            Console.Write(parentDir + "\n");

            rawBrain = cwd + @"\rawBrainDir";
            rawBody = cwd + @"\rawBodyDir";
            upSampledBody = cwd + @"\upSampledBody\";
            upSampledBrain = cwd + @"\upSampledBrain\";


        }


        static void Main()
        {

            //Program.Test();

                Program.Options();
                Console.ReadLine();
        }
    }
}
