using System;
using System.Collections.Generic;
using UnityServer.Models;

namespace UnityServer.GameLogic
{
    public class MapGenerator
    {
        const int minSizeOfField=7;
        const int maxSizeOfField = 13;
        const int minPlayersNumber = 1;
        const int maxPlayersNumber = 6;


        public int SizeOfField;
        public int PlayersNumber;


        private int seed;

        //For Shufle
        List<Coord> allTileCoords;
        Queue<Coord> shuffledTiledCoords;
        List<Unit> Units;

        public struct Coord
        {
            public int X;
            public int Y;

            public Coord(int _x, int _y)
            {
                X = _x;
                Y = _y;
            }

        }

        public State GetNewMap()
        {
            SizeOfField = GetRandomNumber(minSizeOfField, maxSizeOfField);
            PlayersNumber = GetRandomNumber(minPlayersNumber, maxPlayersNumber);
            seed = new Random().Next();

            State state = new State();

            allTileCoords = new List<Coord>();
            for (int x = 0; x < SizeOfField; x++)
            {
                for (int y = 0; y < SizeOfField; y++)
                {
                    allTileCoords.Add(new Coord(x, y));
                }
            }

            shuffledTiledCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
            Units = new List<Unit>();
            for (int i = 0; i < PlayersNumber; i++)
            {
                Coord randomCoord = GetRandomCoord();
                Units.Add(new Unit(randomCoord.X, randomCoord.Y));
            }

            foreach (var unit in Units)
            {
                unit.TargetX = unit.X;
                unit.TargetY = unit.Y;
                state.Units.Add(unit);
            }

            state.FieldSize = SizeOfField;
            state.NumberOfPlayer = PlayersNumber;


            return state;
        }

        public int GetRandomNumber(int left, int right)
        {
            Random rnd = new Random();
            return rnd.Next(left, right);

        }

        public Coord GetRandomCoord()
        {
            Coord randomCoord = shuffledTiledCoords.Dequeue();
            shuffledTiledCoords.Enqueue(randomCoord);
            return randomCoord;
        }
    }
}