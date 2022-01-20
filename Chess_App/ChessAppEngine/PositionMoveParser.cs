using System;
using System.Collections.Generic;
using System.Text;

namespace Chess_App
{
    public sealed class PositionMoveParser
    {
        public byte[] ParsedMoves(string source, string target) 
        {
            byte[] moves = new byte[2];

            moves[0] = SingleParse(source);
            moves[1] = SingleParse(target);

            return moves;
        }

        private byte SingleParse(string move) 
        {
            byte column=8;
            byte row;
            byte result=64;
            if (move.Length == 2) 
            {
                string firstModifier = move[0].ToString();
                switch (firstModifier) 
                {
                    case "a":
                        column = 0;
                        break;
                    case "b":
                        column = 1;
                        break;
                    case "c":
                        column = 2;
                        break;
                    case "d":
                        column = 3;
                        break;
                    case "e":
                        column = 4;
                        break;
                    case "f":
                        column = 5;
                        break;
                    case "g":
                        column = 6;
                        break;
                    case "h":
                        column = 7;
                        break;
                    default:
                        break;
                }

                Byte.TryParse(move[1].ToString(), out row);

                switch (row) 
                {
                    case 1:
                        row = 7;
                        break; 
                    case 2:
                        row = 6;
                        break;
                    case 3:
                        row = 5;
                        break;
                    case 4:
                        row = 4;
                        break;
                    case 5:
                        row = 3;
                        break;
                    case 6:
                        row = 2;
                        break;
                    case 7:
                        row = 1;
                        break;
                    case 8:
                        row = 0;
                        break;
                }

                if(column < 8 && row < 8) 
                {
                    result = (byte)(row * 8 + column);
                    if (result < 64) 
                    {
                        return result;
                    }
                }
            }

            return result;
        }
    }
}
