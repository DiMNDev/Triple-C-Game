// See https://aka.ms/new-console-template for more information
using Chess_Final.Chess;
Console.WriteLine("Hello, World!");


Console.Clear();

Chess chessGame = new Chess();

chessGame.LayoutGamePieces();

// var playerPieces = data.SelectMany(p => p.player
//                        .SelectMany(g => typeof(PieceGroup).GetProperties()
//                        .SelectMany(prop =>
//                        (prop.GetValue(g) as IEnumerable<Piece> ?? Enumerable.Empty<Piece>())
//                        .Select(piece =>
//                         {

//                             GamePiece newPiece = prop.Name switch
//                             {
//                                 "pawns" => new ChessPieces.Pawn { Type = PieceType.pawn, CurrentPosition = (piece.x, piece.y) },
//                                 "rooks" => new ChessPieces.Rook { Type = PieceType.rook, CurrentPosition = (piece.x, piece.y) },
//                                 "knights" => new ChessPieces.Knight { Type = PieceType.knight, CurrentPosition = (piece.x, piece.y) },
//                                 "bishops" => new ChessPieces.Bishop { Type = PieceType.bishop, CurrentPosition = (piece.x, piece.y) },
//                                 "queen" => new ChessPieces.Queen { Type = PieceType.queen, CurrentPosition = (piece.x, piece.y) },
//                                 "king" => new ChessPieces.King { Type = PieceType.king, CurrentPosition = (piece.x, piece.y) },
//                             };
//                             return newPiece;
//                         }
//                         ))));
// PlayerOne.GamePieces = playerPieces;
// foreach (var piece in playerPieces)
// {
//     if (piece != null)
//     {
//         Console.WriteLine($"{piece.Name} @ ({piece.CurrentPosition.X},{piece.CurrentPosition.Y})");
//     }
//     else
//     {
//         Console.WriteLine("null");
//     }
// }