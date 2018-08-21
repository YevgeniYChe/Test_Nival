using System;
using System.Collections.Generic;
using System.Linq;
using UnityServer.Models;

namespace UnityServer.GameLogic
{
    public class Location
    {
        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;
        public Location Parent;
    }

    public class PathFinder
    {

        public int[,] Map;

        public void MarkerUnit(Unit unit)
        {
            unit.IsMoving = false;
            unit.TargetX = unit.X;
            unit.TargetY = unit.Y;
        }

        public State GetPath(State state)
        {
            Map = new int[state.FieldSize, state.FieldSize];

            for(int i=0;i<state.FieldSize;i++)
            {
                for(int t=0;t<state.FieldSize;t++)
                {
                    Map[i, t] = 0;
                }
            }
            foreach(var Unit in state.Units)
            {
                Map[Unit.X, Unit.Y] = 1;
            }
            return Path(state,Map);
        }

        public State Path(State state, int[,] Map)
        {
            var UpdateState = new State();
            UpdateState.FieldSize = state.FieldSize;
            UpdateState.NumberOfPlayer = state.NumberOfPlayer;
            foreach (var unit in state.Units)
            {
                if (unit.IsSelect || unit.IsMoving)
                {
                    if (unit.X == unit.TargetX && unit.Y == unit.TargetY)
                    {
                        unit.IsMoving = false;
                        UpdateState.Units.Add(unit);

                        continue;
                    }

                    var NearTargetPoints = GetWalkableAdjacentSquares(unit.TargetX, unit.TargetY, state.FieldSize, Map);
                    var H = ComputeHScore(unit.X, unit.Y, unit.TargetX, unit.TargetY);

                    if (H == 1 && Map[unit.TargetX, unit.TargetY] == 1)
                    {
                        MarkerUnit(unit);
                        UpdateState.Units.Add(unit);

                        continue;
                    }

                    if ((H ==2 && NearTargetPoints.Count == 0) && Map[unit.TargetX, unit.TargetY] == 1)
                    {
                        MarkerUnit(unit);
                        UpdateState.Units.Add(unit);

                        continue;
                    }
                    
                    Location current = null;
                    var start = new Location { X = unit.X, Y = unit.Y };
                    var target = new Location { X = unit.TargetX, Y = unit.TargetY };
                    var openList = new List<Location>();
                    var closedList = new List<Location>();
                    int g = 0;

                    openList.Add(start);

                    while ((openList.Count > 0 && openList.Count < 100))
                    {

                        var lowest = openList.Min(l => l.F);
                        current = openList.First(l => l.F == lowest);

                        closedList.Add(current);

                        openList.Remove(current);


                        if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null)
                            break;

                        H = ComputeHScore(current.X, current.Y, target.X, target.Y);
                        if (H == 1 && Map[target.X, target.Y] == 1)
                        {
                            openList.Clear();
                            closedList.Clear();
                            break;
                        }
                        if ((H == 2 && NearTargetPoints.Count == 0) && Map[unit.TargetX, unit.TargetY] == 1)
                        {
                            openList.Clear();
                            closedList.Clear();

                            break;
                        }
                        else
                        {
                            var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, state.FieldSize, Map);
                            g++;

                            foreach (var adjacentSquare in adjacentSquares)
                            {
                                if (!closedList.Any(l => l.X == adjacentSquare.X && l.Y == adjacentSquare.Y))
                                {
                                    if (!openList.Any(l => l.X == adjacentSquare.X && l.Y == adjacentSquare.Y))
                                    {
                                        adjacentSquare.G = g;
                                        adjacentSquare.H = ComputeHScore(adjacentSquare.X, adjacentSquare.Y, target.X, target.Y);
                                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                                        adjacentSquare.Parent = current;

                                        openList.Insert(0, adjacentSquare);
                                    }
                                    else
                                    {
                                        if (g + adjacentSquare.H < adjacentSquare.F)
                                        {
                                            adjacentSquare.G = g;
                                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                                        }
                                    }
                                }

                            }
                        }
                    }

                    while (current != null)
                    {
                        if (current.G == 1)
                        {
                            Map[unit.X, unit.Y] = 0;
                            unit.X = current.X;
                            unit.Y = current.Y;
                            unit.IsMoving = true;
                            Map[current.X, current.Y] = 1;

                        }
                        current = current.Parent;

                    }

                    UpdateState.Units.Add(unit);
                }
                else
                {
                    UpdateState.Units.Add(unit);
                }
            }
            return UpdateState;
        }

        static List<Location> GetWalkableAdjacentSquares(int x, int y, int n, int[,] map)
        {
            var list = new List<Location>();
            var proposedLocations = new List<Location>()
            {
                new Location { X = x, Y = y - 1 },
                new Location { X = x, Y = y + 1 },
                new Location { X = x - 1, Y = y },
                new Location { X = x + 1, Y = y },
            };
            foreach (var v in proposedLocations)
            {
                if ((v.Y < n && v.Y >= 0) && (v.X < n && v.X >= 0))
                {
                    list.Add(v);
                }
            }

            return list.Where(l => map[l.X, l.Y] == 0).ToList();
        }

        static int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }

    }
}