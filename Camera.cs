using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using System.Diagnostics;
using GrapplingGame.GameObjectsComponentsLevels.Helpers;

namespace GrapplingGame
{
    public class Camera
    {
        public static Camera Instance;

        GameManager currentGM;

        public Vector2 CurrentCenter = new(800, 450);
        public Vector2 TargetCenter = new(800, 450);

        public float CameraSpeed = 15;

        public CAMERAMODE cameraMode = CAMERAMODE.playerCenter;

        public Camera()
        {
            Instance ??= this;

            currentGM = GameManager.Instance;
        }

        public void Update(GameTime gameTime)
        {
            // Move towards targetCenter
            if (CurrentCenter != TargetCenter)
            {
                int targetDistance = (int)Math.Floor(Math.Sqrt(Math.Pow(TargetCenter.X - CurrentCenter.X, 2) + Math.Pow(TargetCenter.Y - CurrentCenter.Y, 2)));
                Vector2 targetDistancePoint = Vector2.Normalize(new(TargetCenter.X - CurrentCenter.X, TargetCenter.Y - CurrentCenter.Y));

                if (targetDistance >= CameraSpeed)
                {
                    Move(targetDistancePoint * new Vector2(CameraSpeed, CameraSpeed));
                } else
                {
                    Move(targetDistancePoint * new Vector2(targetDistance, targetDistance));
                }
            }

            if (currentGM.CurrentLevel.Player != null)
            {
                switch (cameraMode)
                {
                    case CAMERAMODE.playerCenter:
                        DefaultCameraMovement();
                        break;
                    case CAMERAMODE.playerLeft:
                        CameraLeft();
                        break;
                    case CAMERAMODE.playerRight:
                        CameraRight();
                        break;
                    case CAMERAMODE.playerBottom:
                        CameraBottom();
                        break;
                    case CAMERAMODE.playerTop:
                        CameraTop();
                        break;
                    default:
                        break;
                }
            } else
            {
                TargetCenter = new Vector2(currentGM.HalfScreenWidth, currentGM.HalfScreenHeight); 
            }
        }

        public void Move(Vector2 movementAmount)
        {
            currentGM.OrthographicCamera.Move(movementAmount);

            CurrentCenter += movementAmount;
        }

        void DefaultCameraMovement()
        {
            if (currentGM.CurrentLevel.Player.position.X > currentGM.HalfScreenWidth)
            {
                TargetCenter = new Vector2(currentGM.CurrentLevel.Player.position.X + 100, currentGM.HalfScreenHeight);
            } else
            {
                TargetCenter = new Vector2(currentGM.HalfScreenWidth, currentGM.HalfScreenHeight);
            }
        }

        void CameraLeft()
        {
            if (currentGM.CurrentLevel.Player.position.X > 50)
            {
                TargetCenter = new Vector2(currentGM.CurrentLevel.Player.position.X + currentGM.HalfScreenWidth - 50, currentGM.HalfScreenHeight);
            } else
            {
                TargetCenter = new Vector2(currentGM.HalfScreenWidth, currentGM.HalfScreenHeight);
            }
        }

        void CameraRight()
        {
            if (currentGM.CurrentLevel.Player.position.X > currentGM.HalfScreenWidth * 2 - 100)
            {
                TargetCenter = new Vector2(currentGM.CurrentLevel.Player.position.X - currentGM.HalfScreenWidth + 100, currentGM.HalfScreenHeight);
            }
            else
            {
                TargetCenter = new Vector2(currentGM.HalfScreenWidth, currentGM.HalfScreenHeight);
            }
        }

        void CameraBottom()
        {
            if (currentGM.CurrentLevel.Player.position.Y > currentGM.HalfScreenHeight * 2 - 100)
            {
                TargetCenter = new Vector2(currentGM.HalfScreenWidth, currentGM.CurrentLevel.Player.position.Y - currentGM.HalfScreenHeight + 100);
            }
            else
            {
                TargetCenter = new Vector2(currentGM.HalfScreenWidth, currentGM.HalfScreenHeight);
            }
        }

        void CameraTop()
        {
            if (currentGM.CurrentLevel.Player.position.Y > 50)
            {
                TargetCenter = new Vector2(currentGM.HalfScreenWidth, currentGM.CurrentLevel.Player.position.Y + currentGM.HalfScreenHeight - 50);
            }
            else
            {
                TargetCenter = new Vector2(currentGM.HalfScreenWidth, currentGM.HalfScreenHeight);
            }
        }
    }
}