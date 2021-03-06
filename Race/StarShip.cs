﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.CompilerServices;
using System.Windows.Media.Animation;

namespace Race
{
    public class StarShip : INotifyPropertyChanged
    {
        MainWindow _mainWindow;
        public Rectangle shipRectangle;
        public Rectangle ShipRectangle
        {
            get { return shipRectangle; }
            set
            {
                shipRectangle = value;
                OnPropertyChanged("ShipRectangle");
            }
        }

        public Rect ShipHitBox;

        public bool ShowHitBoxes = false;

        double health_point;
        double ammo_point;
        int ship_score = 0;
        public double ShipHp
        {
            get { return health_point; }
            set
            {
                health_point = value;
                OnPropertyChanged("ShipHp");
            }
        }
        public double ShipAmmo
        {
            get { return ammo_point; }
            set
            {
                ammo_point = value;
                OnPropertyChanged("ShipAmmo");
            }
        }
        public int ShipScore
        {
            get { return ship_score; }
            set
            {
                ship_score = value;
                OnPropertyChanged("ShipScore");
            }
        }


        public List<Ellipse> CurrentAmmos = null;


        double rotate_speed = 5;
        double movement_speed = 3;

        double Angle = 0;

        public StarShip(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            health_point = 100;
            ammo_point = 250;
            CurrentAmmos = new List<Ellipse>();
            ShipRectangle = new Rectangle();
            VisualBrush vb = new VisualBrush();
            vb.Visual = (Visual)Application.Current.Resources["falcone"];
            ShipRectangle.Fill = vb;
            ShipRectangle.Name = "Millenniumfalcon";
            ShipRectangle.Width = 50;
            ShipRectangle.Height = 60;
            ShipRectangle.Margin = new Thickness(_mainWindow.MainCanvas.ActualWidth / 2 - ShipRectangle.Width / 2,
                _mainWindow.MainCanvas.ActualHeight - ShipRectangle.Height - 25,
                0,0);
            _mainWindow.MainCanvas.Children.Add(ShipRectangle);

            ShipHitBox.Width = ShipRectangle.Width * 0.7;
            ShipHitBox.Height = ShipRectangle.Height * 0.7;
            ShipHitBox.X = ShipRectangle.Margin.Left + ((ShipRectangle.Width - ShipHitBox.Width) / 2);
            ShipHitBox.Y = ShipRectangle.Margin.Top + ((ShipRectangle.Height - ShipHitBox.Height) / 2);

            this.PropertyChanged += StarShipPropertyChanged;

            if (ShowHitBoxes)
            {
                Rectangle hb = new Rectangle();
                hb.Name = "HitRectangle";
                hb.Margin = new Thickness(ShipHitBox.X,
                    ShipHitBox.Y,
                    0, 0);
                hb.Width = ShipHitBox.Width;
                hb.Height = ShipHitBox.Height;
                hb.Fill = Brushes.LightGreen;
                _mainWindow.MainCanvas.Children.Add(hb);
            }
        }

        public void ShipFire()
        {
            if (ShipAmmo <= 0) return;
            double coord_x = Math.Cos(Math.PI / 180 * (Angle + 90));
            double coord_y = Math.Cos(Math.PI / 180 * (Angle));
            Ellipse fire = new Ellipse();
            fire.Width = 3; fire.Height = 3;
            fire.Fill = Brushes.Red;
            fire.Name = "bullet";
            fire.Margin = new Thickness(
                ShipRectangle.Margin.Left + ShipRectangle.Width / 2,
                ShipRectangle.Margin.Top + ShipRectangle.Height / 2,
                ShipRectangle.Margin.Right,
                ShipRectangle.Margin.Bottom
                );
            (Application.Current.MainWindow as MainWindow).MainCanvas.Children.Add(fire);
            AnimationsRace.AnimationBulletfire(fire, shipRectangle, coord_x, coord_y);
            CurrentAmmos.Add(fire);
            ShipAmmo--;
        }
        public Rect GetHitBoxFire(Ellipse fire_hb)
        {
            Rect FireHitBox = new Rect();
            FireHitBox.Width = fire_hb.Width;
            FireHitBox.Height = fire_hb.Height;
            FireHitBox.X = fire_hb.Margin.Left;
            FireHitBox.Y = fire_hb.Margin.Top;
            return FireHitBox;
        }

        public void ShipUp()
        {
            double diff_angle = 90;
            //if ((Angle > 270 && Angle <= 360) || (Angle >= 0 && Angle < 90))
            //    diff_angle = 90;
            ////else if (Angle < 270 && Angle >=180)
            ////    diff_angle = 180;
            ////else if (Angle >= 90 && Angle < 180)
            ////    diff_angle = 90;

            double coord_x = Math.Cos(Math.PI / 180 * (Angle + diff_angle)) * movement_speed;
            double coord_y = Math.Cos(Math.PI / 180 * (Angle)) * movement_speed;
            
            Thickness coords = new Thickness(
                ShipRectangle.Margin.Left - coord_x,
                ShipRectangle.Margin.Top - coord_y,
                ShipRectangle.Margin.Right,
                ShipRectangle.Margin.Bottom
                );
            ShipRectangle.Margin = coords;
            ShipRectangle = shipRectangle;

        }
        public void ShipDown()
        {
            double coord_x = Math.Cos(Math.PI / 180 * (Angle + 90)) * movement_speed;
            double coord_y = Math.Cos(Math.PI / 180 * (Angle)) * movement_speed;

            Thickness coords = new Thickness(
                ShipRectangle.Margin.Left + coord_x,
                ShipRectangle.Margin.Top + coord_y,
                ShipRectangle.Margin.Right,
                ShipRectangle.Margin.Bottom
                );
            ShipRectangle.Margin = coords;
            ShipRectangle = shipRectangle;
        }
        public void ShipRight()
        {
            Angle = Angle + rotate_speed >= 360 ? 0 : Angle + rotate_speed;
            RotateTransform myRotateTransform = new RotateTransform();
            myRotateTransform.Angle = Angle;
            myRotateTransform.CenterX = ShipRectangle.Width / 2;
            myRotateTransform.CenterY = ShipRectangle.Height / 2;
            ShipRectangle.RenderTransform = myRotateTransform;
            ShipRectangle = shipRectangle;
        }
        public void ShipLeft()
        {
            Angle = Angle - rotate_speed <= 0 ? 360 : Angle - rotate_speed;
            RotateTransform myRotateTransform = new RotateTransform();
            myRotateTransform.Angle = Angle;
            myRotateTransform.CenterX = ShipRectangle.Width / 2;
            myRotateTransform.CenterY = ShipRectangle.Height / 2;
            ShipRectangle.RenderTransform = myRotateTransform;
            ShipRectangle = shipRectangle;
        }

        private void StarShipPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ShipHitBox.X = (sender as StarShip).shipRectangle.Margin.Left + (((sender as StarShip).shipRectangle.Width - ShipHitBox.Width) / 2);
            ShipHitBox.Y = (sender as StarShip).shipRectangle.Margin.Top + (((sender as StarShip).shipRectangle.Height - ShipHitBox.Height) / 2);
            if (ShowHitBoxes)
            {
                foreach (UIElement item in _mainWindow.MainCanvas.Children)
                {
                    if((item is Rectangle) && (item as Rectangle).Name == "HitRectangle")
                    {
                        _mainWindow.MainCanvas.Children.Remove(item);
                        break;
                    }
                }
                Rectangle hb = new Rectangle();
                hb.Name = "HitRectangle";
                hb.Margin = new Thickness(ShipHitBox.X,
                    ShipHitBox.Y,
                    0, 0);
                hb.Width = ShipHitBox.Width;
                hb.Height = ShipHitBox.Height;
                hb.Fill = Brushes.LightGreen;
                _mainWindow.MainCanvas.Children.Add(hb);
            }
            else
            {
                foreach (UIElement item in _mainWindow.MainCanvas.Children)
                {
                    if ((item is Rectangle) && (item as Rectangle).Name == "HitRectangle")
                    {
                        _mainWindow.MainCanvas.Children.Remove(item);
                        break;
                    }
                }
            }
        }

        #region INotifyPropertyChanged requirements
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
