using System.Collections.Generic;

namespace Chess_App
{
    internal static class ChessPieceValidMoves
    {
        private static void AnalyzeMovePawn(ChessBoard board, byte dstPos, ChessPiece pcMoving)
        {
            //Because Pawns only kill diagonaly we handle the En Passant scenario specialy
            if (board.EnPassantPosition > 0)
            {
                if (pcMoving.PieceColor != board.EnPassantColor)
                {
                    if (board.EnPassantPosition == dstPos)
                    {
                        //We have an En Passant Possible
                        pcMoving.ValidMoves.Push(dstPos);

                        if (pcMoving.PieceColor == ChessPieceColor.White)
                        {
                            board.WhiteAttackedSquares[dstPos] = true;
                        }
                        else
                        {
                            board.BlackAttackedSquares[dstPos] = true;
                        }
                    }
                }
            }

            ChessPiece pcAttacked = board.Squares[dstPos].Piece;

            //If there no piece there I can potentialy kill
            if (pcAttacked == null)
                return;

            //Regardless of what is there I am attacking this square
            if (pcMoving.PieceColor == ChessPieceColor.White)
            {
                board.WhiteAttackedSquares[dstPos] = true;

                //if that piece is the same color
                if (pcAttacked.PieceColor == pcMoving.PieceColor)
                {
                    pcAttacked.DefendedValue += pcMoving.PieceActionValue;
                    return;
                }

                pcAttacked.AttackedValue += pcMoving.PieceActionValue;

                //If this is a king set it in check                   
                if (pcAttacked.PieceType == ChessPieceType.King)
                {
                    board.BlackCheck = true;
                }
                else
                {
                    //Add this as a valid move
                    pcMoving.ValidMoves.Push(dstPos);
                }
            }
            else
            {
                board.BlackAttackedSquares[dstPos] = true;

                //if that piece is the same color
                if (pcAttacked.PieceColor == pcMoving.PieceColor)
                {
                    pcAttacked.DefendedValue += pcMoving.PieceActionValue;
                    return;
                }

                pcAttacked.AttackedValue += pcMoving.PieceActionValue;

                //If this is a king set it in check                   
                if (pcAttacked.PieceType == ChessPieceType.King)
                {
                    board.WhiteCheck = true;
                }
                else
                {
                    //Add this as a valid move
                    pcMoving.ValidMoves.Push(dstPos);
                }
            }

            return;
        }

        private static bool AnalyzeMove(ChessBoard board, byte destinationSquare, ChessPiece movingPiece)
        {
            //sprawdzenie czy nie jest pionem
            if (movingPiece.PieceColor == ChessPieceColor.White)
            {
                board.WhiteAttackedSquares[destinationSquare] = true;
            }
            else
            {
                board.BlackAttackedSquares[destinationSquare] = true;
            }

            //Je?li pole jest puste to mage tam wykona? ruch 
            if (board.Squares[destinationSquare].Piece == null)
            {
                movingPiece.ValidMoves.Push(destinationSquare);

                return true;
            }

            ChessPiece attackedPiece = board.Squares[destinationSquare].Piece;

            //Czy to bierka przeciwnika
            if (attackedPiece.PieceColor != movingPiece.PieceColor)
            {
                attackedPiece.AttackedValue += movingPiece.PieceActionValue;

                //Je?li bierka przeciwnika to kr?l to jest to ruch szachuj?cy                   
                if (attackedPiece.PieceType == ChessPieceType.King)
                {
                    if (attackedPiece.PieceColor == ChessPieceColor.Black)
                    {
                        board.BlackCheck = true;
                    }
                    else
                    {
                        board.WhiteCheck = true;
                    }
                }
                else
                {                     
                    movingPiece.ValidMoves.Push(destinationSquare);
                }                
                return false;
            }
            //Je?li to bierka tego samego koloru to jest to obro?ca
            attackedPiece.DefendedValue += movingPiece.PieceActionValue;
            
            return false;
        }

        private static void CheckValidMovesPawn(List<byte> moves, ChessPiece pcMoving, byte srcPosition,
                                                ChessBoard board, byte count)
        {
            for (byte i = 0; i < count; i++)
            {
                byte dstPos = moves[i];

                //Diagonal
                if (dstPos%8 != srcPosition%8)
                {
                    //If there is a piece there I can potentialy kill
                    AnalyzeMovePawn(board, dstPos, pcMoving);

                    if (pcMoving.PieceColor == ChessPieceColor.White)
                    {
                        board.WhiteAttackedSquares[dstPos] = true;
                    }
                    else
                    {
                        board.BlackAttackedSquares[dstPos] = true;
                    }
                }
                // if there is something if front pawns can't move there
                else if (board.Squares[dstPos].Piece != null)
                {
                    return;
                }
                //if there is nothing in front of 
                else
                {
                    pcMoving.ValidMoves.Push(dstPos);
                }
            }
        }

        private static void GenerateValidMovesKing(ChessPiece piece, ChessBoard board, byte srcPosition)
        {
            if (piece == null)
            {
                return;
            }

            for (byte i = 0; i < MoveArrays.KingTotalMoves[srcPosition]; i++)
            {
                byte dstPos = MoveArrays.KingMoves[srcPosition].Moves[i];

                if (piece.PieceColor == ChessPieceColor.White)
                {
                    //I can't move where I am being attacked
                    if (board.BlackAttackedSquares[dstPos])
                    {
                        board.WhiteAttackedSquares[dstPos] = true;
                        continue;
                    }
                }
                else
                {
                    if (board.WhiteAttackedSquares[dstPos])
                    {
                        board.BlackAttackedSquares[dstPos] = true;
                        continue;
                    }
                }

                AnalyzeMove(board, dstPos, piece);
            }
        }

        private static void GenerateValidMovesKingCastle(ChessBoard board, ChessPiece king)
        {
            //This code will add the castleling move to the pieces available moves
            if (king.PieceColor == ChessPieceColor.White)
            {
                if (board.Squares[63].Piece != null)
                {
                    //Check if the Right Rook is still in the correct position
                    if (board.Squares[63].Piece.PieceType == ChessPieceType.Rook)
                    {
                        if (board.Squares[63].Piece.PieceColor == king.PieceColor)
                        {
                            //Move one column to right see if its empty
                            if (board.Squares[62].Piece == null)
                            {
                                if (board.Squares[61].Piece == null)
                                {
                                    if (board.BlackAttackedSquares[61] == false &&
                                        board.BlackAttackedSquares[62] == false)
                                    {
                                        //Ok looks like move is valid lets add it
                                        king.ValidMoves.Push(62);
                                        board.WhiteAttackedSquares[62] = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (board.Squares[56].Piece != null)
                {
                    //Check if the Left Rook is still in the correct position
                    if (board.Squares[56].Piece.PieceType == ChessPieceType.Rook)
                    {
                        if (board.Squares[56].Piece.PieceColor == king.PieceColor)
                        {
                            //Move one column to right see if its empty
                            if (board.Squares[57].Piece == null)
                            {
                                if (board.Squares[58].Piece == null)
                                {
                                    if (board.Squares[59].Piece == null)
                                    {
                                        if (board.BlackAttackedSquares[58] == false &&
                                            board.BlackAttackedSquares[59] == false)
                                        {
                                            //Ok looks like move is valid lets add it
                                            king.ValidMoves.Push(58);
                                            board.WhiteAttackedSquares[58] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (king.PieceColor == ChessPieceColor.Black)
            {
                //There are two ways to castle, scenario 1:
                if (board.Squares[7].Piece != null)
                {
                    //Check if the Right Rook is still in the correct position
                    if (board.Squares[7].Piece.PieceType == ChessPieceType.Rook
                        && !board.Squares[7].Piece.Moved)
                    {
                        if (board.Squares[7].Piece.PieceColor == king.PieceColor)
                        {
                            //Move one column to right see if its empty

                            if (board.Squares[6].Piece == null)
                            {
                                if (board.Squares[5].Piece == null)
                                {
                                    if (board.WhiteAttackedSquares[5] == false && board.WhiteAttackedSquares[6] == false)
                                    {
                                        //Ok looks like move is valid lets add it
                                        king.ValidMoves.Push(6);
                                        board.BlackAttackedSquares[6] = true;
                                    }
                                }
                            }
                        }
                    }
                }
                //There are two ways to castle, scenario 2:
                if (board.Squares[0].Piece != null)
                {
                    //Check if the Left Rook is still in the correct position
                    if (board.Squares[0].Piece.PieceType == ChessPieceType.Rook &&
                        !board.Squares[0].Piece.Moved)
                    {
                        if (board.Squares[0].Piece.PieceColor ==
                            king.PieceColor)
                        {
                            //Move one column to right see if its empty
                            if (board.Squares[1].Piece == null)
                            {
                                if (board.Squares[2].Piece == null)
                                {
                                    if (board.Squares[3].Piece == null)
                                    {
                                        if (board.WhiteAttackedSquares[2] == false &&
                                            board.WhiteAttackedSquares[3] == false)
                                        {
                                            //Ok looks like move is valid lets add it
                                            king.ValidMoves.Push(2);
                                            board.BlackAttackedSquares[2] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static void GenerateValidMoves(ChessBoard board)
        {
            // Reset Board
            board.BlackCheck = false;
            board.WhiteCheck = false;

            byte blackRooksMoved = 0;
            byte whiteRooksMoved = 0;

            //Calculate Remaining Material on Board to make the End Game Decision
            int remainingPieces = 0;

            //Generate Moves
            for (byte x = 0; x < 64; x++)
            {
                VariableSquare sqr = board.Squares[x];

                if (sqr.Piece == null)
                    continue;

                sqr.Piece.ValidMoves = new Stack<byte>(sqr.Piece.LastValidMoveCount);

                remainingPieces++;

                switch (sqr.Piece.PieceType)
                {
                    case ChessPieceType.Pawn:
                        {
                            if (sqr.Piece.PieceColor == ChessPieceColor.White)
                            {
                                CheckValidMovesPawn(MoveArrays.WhitePawnMoves[x].Moves, sqr.Piece, x,
                                                    board, MoveArrays.MovesListWhitePawn[x]);
                                break;
                            }
                            if (sqr.Piece.PieceColor == ChessPieceColor.Black)
                            {
                                CheckValidMovesPawn(MoveArrays.BlackPawnMoves[x].Moves, sqr.Piece, x,
                                                    board, MoveArrays.MovesListBlackPawn[x]);
                                break;
                            }

                            break;
                        }
                    case ChessPieceType.Knight:
                        {
                            for (byte i = 0; i < MoveArrays.KnightTotalMoves[x]; i++)
                            {
                                AnalyzeMove(board, MoveArrays.KnightMoves[x].Moves[i], sqr.Piece);
                            }

                            break;
                        }
                    case ChessPieceType.Bishop:
                        {
                            for (byte i = 0; i < MoveArrays.BishopTotalMoves1[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.BishopMoves1[x].Moves[i],
                                                sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.BishopTotalMoves2[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.BishopMoves2[x].Moves[i],
                                                sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.BishopTotalMoves3[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.BishopMoves3[x].Moves[i],
                                                sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.BishopTotalMoves4[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.BishopMoves4[x].Moves[i],
                                                sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }

                            break;
                        }
                    case ChessPieceType.Rook:
                        {
                            if (sqr.Piece.Moved)
                            {
                                if (sqr.Piece.PieceColor == ChessPieceColor.Black)
                                {
                                    blackRooksMoved++;
                                }
                                else
                                {
                                    whiteRooksMoved++;
                                }
                            }


                            for (byte i = 0; i < MoveArrays.RookTotalMoves1[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.RookMoves1[x].Moves[i], sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.RookTotalMoves2[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.RookMoves2[x].Moves[i], sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.RookTotalMoves3[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.RookMoves3[x].Moves[i], sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.RookTotalMoves4[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.RookMoves4[x].Moves[i], sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }

                            break;
                        }
                    case ChessPieceType.Queen:
                        {
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves1[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves1[x].Moves[i], sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves2[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves2[x].Moves[i], sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves3[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves3[x].Moves[i], sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves4[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves4[x].Moves[i], sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }

                            for (byte i = 0; i < MoveArrays.QueenTotalMoves5[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves5[x].Moves[i], sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves6[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves6[x].Moves[i], sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves7[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves7[x].Moves[i], sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves8[x]; i++)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves8[x].Moves[i], sqr.Piece) ==
                                    false)
                                {
                                    break;
                                }
                            }

                            break;
                        }
                    case ChessPieceType.King:
                        {
                            if (sqr.Piece.PieceColor == ChessPieceColor.White)
                            {
                                if (sqr.Piece.Moved)
                                {
                                    board.WhiteCanCastle = false;
                                }
                                board.WhiteKingPosition = x;
                            }
                            else
                            {
                                if (sqr.Piece.Moved)
                                {
                                    board.BlackCanCastle = false;
                                }
                                board.BlackKingPosition = x;
                            }

                            break;
                        }
                }
            }

            if (blackRooksMoved > 1)
            {
                board.BlackCanCastle = false;
            }
            if (whiteRooksMoved > 1)
            {
                board.WhiteCanCastle = false;
            }

            if (remainingPieces < 10)
            {
                board.EndGamePhase = true;
            }


            if (board.WhoseMove == ChessPieceColor.White)
            {
                GenerateValidMovesKing(board.Squares[board.BlackKingPosition].Piece, board,
                                       board.BlackKingPosition);
                GenerateValidMovesKing(board.Squares[board.WhiteKingPosition].Piece, board,
                                       board.WhiteKingPosition);
            }
            else
            {
                GenerateValidMovesKing(board.Squares[board.WhiteKingPosition].Piece, board,
                                       board.WhiteKingPosition);
                GenerateValidMovesKing(board.Squares[board.BlackKingPosition].Piece, board,
                                       board.BlackKingPosition);
            }


            //Now that all the pieces were examined we know if the king is in check
            if (!board.WhiteCastled && board.WhiteCanCastle && !board.WhiteCheck)
            {
                GenerateValidMovesKingCastle(board, board.Squares[board.WhiteKingPosition].Piece);
            }
            if (!board.BlackCastled && board.BlackCanCastle && !board.BlackCheck)
            {
                GenerateValidMovesKingCastle(board, board.Squares[board.BlackKingPosition].Piece);
            }
        }
    }
}