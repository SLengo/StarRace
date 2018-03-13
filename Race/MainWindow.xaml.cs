﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Race
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer ObstsGeneratorTimer;
        DispatcherTimer CollisionTimer;
        DispatcherTimer KeyTrackTimer;
        DispatcherTimer BonusGeneratorTimer;
        BetterRandom RandForSomethings;

        public StarShip ship;
        public Stars stars;

        public List<Obstacle> CurrentObsts = null;
        public List<Bonus> CurrentBonuses = null;

        bool leftpress = false;
        bool rightpress = false;
        bool uppress = false;
        bool downpress = false;
        bool spacepress = false;

        public MainWindow()
        {
            InitializeComponent();

            CurrentObsts = new List<Obstacle>();
            CurrentBonuses = new List<Bonus>();
            RandForSomethings = new BetterRandom();

            BonusGeneratorTimer = new DispatcherTimer();
            BonusGeneratorTimer.Interval = TimeSpan.FromMilliseconds(1000);
            BonusGeneratorTimer.Tick += new EventHandler(BonusGeneratorTimerTick);

            ObstsGeneratorTimer = new DispatcherTimer();
            ObstsGeneratorTimer.Interval = TimeSpan.FromMilliseconds(3500);
            ObstsGeneratorTimer.Tick += new EventHandler(ObstsGeneratorTimerTick);

            KeyTrackTimer = new DispatcherTimer();
            KeyTrackTimer.Interval = TimeSpan.FromMilliseconds(8);
            KeyTrackTimer.Tick += new EventHandler(KeyTrackTimerTimerTick);

            CollisionTimer = new DispatcherTimer();
            CollisionTimer.Interval = TimeSpan.FromMilliseconds(8);
            CollisionTimer.Tick += new EventHandler(CollisionTimerTimerTick);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConsoleMethod.WriteToConsole("Game window loaded", Brushes.White);
            stars = new Stars(this);
        }

        private void InitGame()
        {
            ship = new StarShip(this);
            this.DataContext = ship;
            BonusGeneratorTimer.Start();
            ObstsGeneratorTimer.Start();
            KeyTrackTimer.Start();
            CollisionTimer.Start();
        }

        private void ObstsGeneratorTimerTick(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                Obstacle obst = new Obstacle(this);
                CurrentObsts.Add(obst);
                AnimationsRace.AnimationObstacles(obst);
            }
        }

        private void BonusGeneratorTimerTick(object sender, EventArgs e)
        {
            if(RandForSomethings.Between(0,5) == 1)
            {
                if (RandForSomethings.Between(0, 1) == 0)
                {
                    AmmoBonus bonus = new AmmoBonus(this);
                    CurrentBonuses.Add(bonus);
                    AnimationsRace.AnimationBonus(bonus);
                }
                else
                {
                    HealthBonus bonus = new HealthBonus(this);
                    CurrentBonuses.Add(bonus);
                    AnimationsRace.AnimationBonus(bonus);
                }
            }
        }

        private void CollisionTimerTimerTick(object sender, EventArgs e)
        {
            // check collision obsts and ship
            for (int i = 0; i < CurrentObsts.Count; i++)
            {
                if (ship.ShipHitBox.IntersectsWith(CurrentObsts[i].GetHitBoxObst()))
                {
                    ConsoleMethod.WriteToConsole("Obst number " + i + " hitted!", Brushes.White);
                    ship.ShipHp -= CurrentObsts[i].ObstDamage;
                }
            }

            // check collision bullets and obsts
            for (int i = 0; i < ship.CurrentAmmos.Count; i++)
            {
                for (int j = 0; j < CurrentObsts.Count; j++)
                {
                    if (CurrentObsts[j].GetHitBoxObst().IntersectsWith(
                        ship.GetHitBoxFire(ship.CurrentAmmos[i])) &&
                        !CurrentObsts[j].Hitted)
                    {
                        ConsoleMethod.WriteToConsole("Obst number " + j + " fired by bullet number "+ i + "!", Brushes.White);
                        CurrentObsts[j].Hitted = true;
                        CurrentObsts[j].ObstacleFiredAnimation();
                        RemoveElementAfterAnimation(CurrentObsts[j].ObstToCanvas);
                        CurrentObsts.Remove(CurrentObsts[j]);
                    }
                }
            }

            // check collision with bonuses
            for (int i = 0; i < CurrentBonuses.Count; i++)
            {
                if(ship.ShipHitBox.IntersectsWith(CurrentBonuses[i].GetHitBoxObst()) && !CurrentBonuses[i].Hitted)
                {
                    CurrentBonuses[i].Hitted = true;
                    if (CurrentBonuses[i] is AmmoBonus)
                    {
                        ConsoleMethod.WriteToConsole("Ammo Bonus obtained!", Brushes.White);
                        ship.ShipAmmo += (CurrentBonuses[i] as AmmoBonus).ammo_count;
                    }
                    else if (CurrentBonuses[i] is HealthBonus)
                    {
                        ConsoleMethod.WriteToConsole("HP Bonus obtained!", Brushes.White);
                        if (ship.ShipHp < 100)
                            ship.ShipHp = ship.ShipHp + (CurrentBonuses[i] as HealthBonus).health_count >= 100 ?
                                100 : ship.ShipHp + (CurrentBonuses[i] as HealthBonus).health_count;
                        else
                            continue;
                    }
                    RemoveElementAfterAnimation(CurrentBonuses[i].bonus_rectangle);
                    CurrentBonuses.Remove(CurrentBonuses[i]);
                }
            }
        }

        public void RemoveElementAfterAnimation(UIElement element)
        {
            MainCanvas.Children.Remove(element);
        }

        private void KeyTrackTimerTimerTick(object sender, EventArgs e)
        {
            if (leftpress)
            {
                ship.ShipLeft();
            }
            else if (rightpress)
            {
                ship.ShipRight();
            }
            else if (uppress)
            {
                ship.ShipUp();
            }
            else if (downpress)
            {
                ship.ShipDown();
            }
            else if (spacepress)
            {
                ship.ShipFire();
            }

            if (leftpress && spacepress)
            {
                ship.ShipFire();
                ship.ShipLeft();
            }
            else if (rightpress && spacepress)
            {
                ship.ShipFire();
                ship.ShipRight();
            }
            else if (uppress && spacepress)
            {
                ship.ShipFire();
                ship.ShipUp();
            }
            else if (downpress && spacepress)
            {
                ship.ShipFire();
                ship.ShipDown();
            }

            if (leftpress && uppress)
            {
                ship.ShipLeft();
                ship.ShipUp();
            }
            else if (rightpress && uppress)
            {
                ship.ShipRight();
                ship.ShipUp();
            }
            else if (leftpress && downpress)
            {
                ship.ShipLeft();
                ship.ShipDown();
            }
            else if (rightpress && downpress)
            {
                ship.ShipRight();
                ship.ShipDown();
            }

            if (leftpress && uppress && spacepress)
            {
                ship.ShipFire();
                ship.ShipLeft();
                ship.ShipUp();
            }
            else if (rightpress && uppress && spacepress)
            {
                ship.ShipFire();
                ship.ShipRight();
                ship.ShipUp();
            }
            else if (leftpress && downpress && spacepress)
            {
                ship.ShipFire();
                ship.ShipLeft();
                ship.ShipDown();
            }
            else if (rightpress && downpress && spacepress)
            {
                ship.ShipFire();
                ship.ShipRight();
                ship.ShipDown();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    {
                        leftpress = true;
                        break;
                    }
                case Key.Up:
                    {
                        uppress = true;
                        break;
                    }
                case Key.Right:
                    {
                        rightpress = true;
                        break;
                    }
                case Key.Down:
                    {
                        downpress = true;
                        break;
                    }
                case Key.Space:
                    {
                        spacepress = true;
                        break;
                    }
                case Key.Oem3:
                    {
                        bool has_console = false;
                        foreach (Window item in Application.Current.Windows)
                        {
                            if (item is Console)
                            {
                                has_console = true;
                                item.Close();
                                break;
                            }
                        }
                        if (!has_console)
                        {
                            Console _console = new Console();
                            _console.Owner = this;
                            _console.Show();
                        }
                        break;
                    }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    {
                        leftpress = false;
                        break;
                    }
                case Key.Up:
                    {
                        uppress = false;
                        break;
                    }
                case Key.Right:
                    {
                        rightpress = false;
                        break;
                    }
                case Key.Down:
                    {
                        downpress = false;
                        break;
                    }
                case Key.Space:
                    {
                        spacepress = false;
                        break;
                    }
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            //MainCanvas.Visibility = Visibility.Visible;
            Menu.Visibility = Visibility.Hidden;
            StatusBar.Visibility = Visibility.Visible;
            InitGame();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
