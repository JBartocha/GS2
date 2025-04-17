using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace GS2
{
    public interface ISnake
    {
        public void Move();
        public void SetMovement(string direction);
        public string GetForbiddenMoveDirection();
        public Point GetSnakeHeadPosition();
    }

    internal class Snake : ISnake
    {
        private bool Growing = false;

        private Point Movement = new Point(1, 0); // Start going right
        private string ForbiddenDirection = "Down";
        private SnakePointsEvent SnakePoints = new SnakePointsEvent();

        public delegate void SnakeEventHandler(object sender, SnakePointsEvent args);
        public event SnakeEventHandler SnakeMoveEvent;


        protected virtual void OnSnakeEvent()
        {
            if (SnakeMoveEvent != null)
                SnakeMoveEvent?.Invoke(this, SnakePoints);
        }

        // EVENT from GRID
        private void OnGridCollisionEvent(object sender, GridCollisionArgs e)
        {
            if (e.BlockType == BlockTypes.FoodBlock)
            {
                Growing = true;
            }
        }

        public Snake(Point StartingPosition)
        {
            SnakePoints.Points.Add(new Point(StartingPosition.X, StartingPosition.Y + 2));
            SnakePoints.Points.Add(new Point(StartingPosition.X, StartingPosition.Y + 1));
            SnakePoints.Points.Add(StartingPosition);
        }


        private void SetForbiddenDirection()
        {
            Point HeadPosition = SnakePoints.Points[SnakePoints.Points.Count - 1];
            Point FirstBodyPart = SnakePoints.Points[SnakePoints.Points.Count - 2];

            switch (HeadPosition.X - FirstBodyPart.X)
            {
                case 1:
                    ForbiddenDirection = "Left";
                    break;
                case -1:
                    ForbiddenDirection = "Right";
                    break;
                case 0:
                    if (HeadPosition.Y - FirstBodyPart.Y == 1)
                        ForbiddenDirection = "Up";
                    else
                        ForbiddenDirection = "Down";
                    break;
            }
        }

        public string GetForbiddenMoveDirection()
        {
            return ForbiddenDirection;
        }

        public Point GetSnakeHeadPosition()
        {
            return new Point(SnakePoints.Points[SnakePoints.Points.Count - 1].X, SnakePoints.Points[SnakePoints.Points.Count - 1].Y);
        }

        public void SetMovement(string direction)
        {
            switch (direction)
            {
                case "Up":
                    Movement.X = 0;
                    Movement.Y = -1;
                    break;
                case "Down":
                    Movement.X = 0;
                    Movement.Y = 1;
                    break;
                case "Left":
                    Movement.X = -1;
                    Movement.Y = 0;
                    break;
                case "Right":
                    Movement.X = 1;
                    Movement.Y = 0;
                    break;
                default:
                    throw new ArgumentException("Invalid direction for Movement in Snake.SetMovement class");
            }
        }

        public void Move()
        {
            SnakePoints.Points.Add(new Point(SnakePoints.Points[SnakePoints.Points.Count - 1].X + Movement.X, SnakePoints.Points[SnakePoints.Points.Count - 1].Y + Movement.Y));
            if (!Growing)
            {
                SnakePoints.Points.RemoveAt(0);
            }
            else
            {
                Growing = false;
            }
            SetForbiddenDirection();
            OnSnakeEvent();
        }
    }
}
