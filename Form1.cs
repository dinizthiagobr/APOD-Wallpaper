﻿using AstronomyPicOfTheDay.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace AstronomyPicOfTheDay
{
    public partial class Form1 : Form
    {
        private Utils utils;

        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private System.Timers.Timer t;

        private PictureOfTheDay p;

        private string API_KEY = "YOUR_NASA_API_KEY";

        public Form1()
        {
            InitializeComponent();

            utils = new Utils();

            utils.addToWindowsStartUp();

            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Show Explanation", ShowExplication);
            trayMenu.MenuItems.Add("Exit", OnExit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "APOD";
            trayIcon.Icon = APOD.Icon.iconbig;

            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs args)
            {
                Exception e = (Exception)args.ExceptionObject;
                MessageBox.Show(e.ToString());
                Environment.Exit(1);
            };

            InitializeRoutine();

            /* run every hour */
            t = new System.Timers.Timer(3600000);        
            t.Elapsed += new ElapsedEventHandler(timer_Tick);
            t.Start();
        }

        private void timer_Tick(object sender, ElapsedEventArgs e)
        {
            InitializeRoutine();
        }

        private void InitializeRoutine()
        {
            if (File.Exists(@"C:\pic.xml"))
                p = utils.ReadFromFile();
            else
            {
                p = new PictureOfTheDay();
                p.date = "10/10/1990";
            }

            string dateNow = DateTime.Now.ToString("dd/MM/yyyy");

            if (utils.CompareDates(p.date, dateNow))
            {
                /* A day has passed, get picture again */
                string url = "https://api.nasa.gov/planetary/apod?hd=True&concept_tags=False&api_key=" + API_KEY;
                string response = utils.SendGet(url);

                if (!string.IsNullOrEmpty(response))
                {
                    var picJson = JsonConvert.DeserializeObject<dynamic>(response);

                    p.url = picJson["hdurl"];
                    p.media_type = picJson["media_type"];
                    p.explanation = picJson["explanation"];
                    p.title = picJson["title"];
                    p.date = DateTime.Now.ToString("dd/MM/yyyy");

                    if (!p.media_type.Equals("image"))
                        return;

                    p.DownloadFromUrl();
                    if (File.Exists(@"C:\APOD.jpg"))
                        utils.SetWallpaper();

                    utils.WriteToFile(p);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
        }

        private void ShowExplication(object sender, EventArgs e)
        {
            ExplicationForm ef = new ExplicationForm(p);
            ef.Show();
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
