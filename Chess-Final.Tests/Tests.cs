namespace Chess_Final.Tests;
using Player;
using FluentAssertions;
using Chess_Final.Generics;
using Chess_Final.Chess;
using Chess_Final.Lobby;
using TC_DataManagerException;
using TC_DataManager;
using Chess_Final.PlayerManager;
using Chess_Final.DB_Manager;

public static class Test_Setup
{
    public static (Player PlayerOne, Player PlayerTwo, Game game) CreateDefaultGame()
    {
        Player PlayerOne = new("P1");
        Guid GameID = LobbyManager.CreateGame(GameType.Chess, PlayerOne);
        Game game = LobbyManager.GetGame(GameType.Chess, GameID);
        Player PlayerTwo = new("P2");
        game.JoinGame(PlayerTwo, JoinAs.Player);

        return (PlayerOne, PlayerTwo, game);
    }
}
public class Player_Tests
{
    [Fact]
    public void ShouldCreateNewPlayer()
    {
        // Arrange
        // Act
        Player player = new Player("Jane");

        // Assert
        player.Should().NotBeNull();
    }
    [Fact]
    public void ShouldHaveCorrectPlayerName()
    {
        // Arrange
        var newPlayerName = "John";
        // Act
        Player player = new Player(newPlayerName);

        // Assert
        player.Username.Should().Be(newPlayerName);
    }

    [Fact]
    public void PlayerShouldHaveGuid()
    {
        // Arrange
        var newPlayerName = "John";
        // Act
        Player player = new Player(newPlayerName);
        var id = player.PlayerID;

        // Assert
        player.PlayerID.Should().Be(id);
        player.PlayerID.Should().NotBeEmpty();
    }



}

public class GamePiece_Tests
{
    public class Pawn_Tests
    {
        [Fact]
        public void ShouldHaveValidMove()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();
            // Act
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("A", 6)).FirstOrDefault();
            (int X, int Y) P1Pawn = Chess.ParsePosition(PlayerOnePawn.CurrentPosition);
            var PlayerOneMoveOne = (0, 4);
            var PlayerOneMoveTwo = (0, 5);
            game.CurrentPlayer.Select(P1Pawn.X, P1Pawn.Y, game, PlayerOne);

            game.NewTurn();

            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("H", 1)).FirstOrDefault();
            (int X, int Y) P2Pawn = Chess.ParsePosition(PlayerTwoPawn.CurrentPosition);
            game.CurrentPlayer.Select(P2Pawn.X, P2Pawn.Y, game, PlayerTwo);
            var PlayerTwoMoveOne = (7, 3);
            var PlayerTwoMoveTwo = (7, 2);
            // Assert
            PlayerOnePawn!.AllowedMovement.Count().Should().Be(2);
            PlayerOnePawn!.AllowedMovement.Should().Contain(PlayerOneMoveOne);
            PlayerOnePawn!.AllowedMovement.Should().Contain(PlayerOneMoveTwo);
            PlayerTwoPawn!.AllowedMovement.Count().Should().Be(2);
            PlayerTwoPawn!.AllowedMovement.Should().Contain(PlayerTwoMoveOne);
            PlayerTwoPawn!.AllowedMovement.Should().Contain(PlayerTwoMoveTwo);

        }
        [Fact]
        public void ShouldMovePlayerOnePieceToValidPosition()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("A", 6)).FirstOrDefault();

            // Act
            PlayerOne.Select(0, 6, game, PlayerOne);
            PlayerOne.MovePiece(0, 4, game);

            // Assert
            PlayerOnePawn.CurrentPosition.Should().Be(("A", 4));
            game.Board.Matrix[0, 4].Should().Be(PlayerOnePawn);
            game.Board.Matrix[0, 6].Should().BeNull();
        }
        [Fact]
        public void ShouldMovePlayerTwoPieceToValidPosition()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();

            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("A", 6)).FirstOrDefault();

            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("A", 1)).FirstOrDefault();

            // Act
            PlayerOne.Select(0, 6, game, PlayerOne);
            PlayerOne.MovePiece(0, 4, game);
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select(0, 1, game, PlayerTwo);
            PlayerTwo.MovePiece(0, 3, game);

            // Assert
            PlayerTwoPawn.CurrentPosition.Should().Be(("A", 3));
            game.Board.Matrix[0, 3].Should().Be(PlayerTwoPawn);
            game.Board.Matrix[0, 1].Should().BeNull();
        }
        [Fact]
        public void PawnShouldAllowPawnToAttack()
        {
            // Arrange

            // Setup Game
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();

            // Setup PlayerOne
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.CurrentPosition == ("C", 6)).FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOnePawn.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOnePawn.CurrentPosition.Y;

            // Setup PlayerTwo
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.CurrentPosition == ("B", 1)).FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoPawn.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoPawn.CurrentPosition.Y;

            // Act
            PlayerOne.Select((int)X, Y, game, PlayerOne);
            PlayerOne.MovePiece(2, 4, game);
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select((int)X2, Y2, game, PlayerTwo);
            PlayerTwo.MovePiece(1, 3, game);
            game.CurrentPlayer = PlayerOne;
            PlayerOne.Select(2, 4, game, PlayerOne);
            PlayerOne.MovePiece(1, 3, game);
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select(1, 3, game, PlayerTwo);
            PlayerTwo.MovePiece(1, 4, game);
            GamePiece? RemovedFromPlay = null;
            for (int i = 0; i < game.Board.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < game.Board.Matrix.GetLength(0); j++)
                {
                    if (game.Board.Matrix[i, j] == PlayerTwoPawn) RemovedFromPlay = game.Board.Matrix[i, j];
                }
            }
            // Assert
            PlayerOnePawn.CurrentPosition.Should().Be(("B", 3));
            PlayerTwoPawn.RemovedFromPlay.Should().BeTrue();
            game.Board.Matrix[1, 3].Should().Be(PlayerOnePawn);
            game.Board.Matrix[1, 4].Should().NotBe(PlayerTwoPawn);
            RemovedFromPlay.Should().BeNull();

        }
    }
    public class Rook_Tests
    {
        [Fact]
        public void RookShouldNotHaveAnyValidMovesForBothPlayers()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();
            // Act
            GamePiece? PlayerOneRook = PlayerOne!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOneRook.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneRook.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);

            game.NewTurn();

            GamePiece? PlayerTwoRook = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoRook.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoRook.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);
            // Assert
            PlayerOneRook!.AllowedMovement.Count().Should().Be(0);
            PlayerTwoRook!.AllowedMovement.Count().Should().Be(0);

        }

        [Fact]
        public void RookShouldHaveMultipleValidMovesForPlayerOne()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            GamePiece? PlayerOneRook = PlayerOne!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            // Act
            PlayerOneRook.CurrentPosition = ("C", 3);
            game.PlaceInMatrix();
            Enum.TryParse<ChessCoordinate>(PlayerOneRook.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneRook.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);
            // Assert
            PlayerOneRook.AllowedMovement.Count().Should().Be(11);

        }
        [Fact]
        public void ShouldHaveMultipleValidMovesForPlayerTwo()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            GamePiece? PlayerTwoRook = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            // Act
            PlayerTwoRook.CurrentPosition = ("C", 3);
            game.PlaceInMatrix();
            Enum.TryParse<ChessCoordinate>(PlayerTwoRook.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerTwoRook.CurrentPosition.Y;
            game.CurrentPlayer = PlayerTwo;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerTwo);
            // Assert
            PlayerTwoRook.AllowedMovement.Count().Should().Be(11);

        }
        [Fact]
        public void ShouldMovePlayerOnePieceToValidPosition()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();
            GamePiece? PlayerOneRook = PlayerOne!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("H", 7)).FirstOrDefault();
            PlayerOneRook.CurrentPosition = ("H", 5);

            // Act
            PlayerOne.Select(7, 5, game, PlayerOne);
            PlayerOne.MovePiece(0, 5, game);

            // Assert
            PlayerOneRook.CurrentPosition.Should().Be(("A", 5));
            game.Board.Matrix[0, 5].Should().Be(PlayerOneRook);
            game.Board.Matrix[7, 5].Should().BeNull();
        }
        [Fact]
        public void ShouldMovePlayerTwoPieceToValidPosition()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();

            GamePiece? PlayerTwoRook = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("H", 0)).FirstOrDefault();
            PlayerTwoRook.CurrentPosition = ("H", 3);

            // Act                    
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select(7, 3, game, PlayerTwo);
            PlayerTwo.MovePiece(0, 3, game);

            // Assert
            PlayerTwoRook.CurrentPosition.Should().Be(("A", 3));
            game.Board.Matrix[0, 3].Should().Be(PlayerTwoRook);
            game.Board.Matrix[7, 3].Should().BeNull();
        }
        [Fact]
        public void RookShouldAllowRookToAttack()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();

            // Setup PlayerOne
            GamePiece? PlayerOneRook = PlayerOne!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("H", 7)).FirstOrDefault();
            PlayerOneRook.CurrentPosition = ("H", 5);
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("F", 6)).FirstOrDefault();
            // Setup PlayerTwo
            GamePiece? PlayerTwoRook = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("H", 0)).FirstOrDefault();
            PlayerTwoRook.CurrentPosition = ("F", 3);
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("H", 1)).FirstOrDefault();


            // Act
            PlayerOne.Select(7, 5, game, PlayerOne);
            PlayerOne.MovePiece(7, 1, game);
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select(5, 3, game, PlayerTwo);
            PlayerTwo.MovePiece(5, 6, game);
            game.CurrentPlayer = PlayerOne;

            GamePiece? PlayerOnePawnRemovedFromPlay = null;
            for (int i = 0; i < game.Board.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < game.Board.Matrix.GetLength(0); j++)
                {
                    if (game.Board.Matrix[i, j] == PlayerOnePawn) PlayerOnePawnRemovedFromPlay = game.Board.Matrix[i, j];
                }
            }
            GamePiece? PlayerTwoPawnRemovedFromPlay = null;
            for (int i = 0; i < game.Board.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < game.Board.Matrix.GetLength(0); j++)
                {
                    if (game.Board.Matrix[i, j] == PlayerTwoPawn) PlayerTwoPawnRemovedFromPlay = game.Board.Matrix[i, j];
                }
            }

            // Assert
            PlayerOneRook.CurrentPosition.Should().Be(("H", 1));
            game.Board.Matrix[7, 1].Should().Be(PlayerOneRook);
            PlayerOnePawnRemovedFromPlay.Should().BeNull();
            PlayerTwoRook.CurrentPosition.Should().Be(("F", 6));
            game.Board.Matrix[5, 6].Should().Be(PlayerTwoRook);
            PlayerTwoPawnRemovedFromPlay.Should().BeNull();
        }
    }
    public class Knight_Tests
    {
        [Fact]
        public void KnightShouldHaveTwoMovesForBothPlayers()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            GamePiece? PlayerOneKnight = PlayerOne!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("B", 7)).FirstOrDefault();
            GamePiece? PlayerTwoKnight = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("B", 0)).FirstOrDefault();
            game.PlaceInMatrix();
            // Act            
            game.CurrentPlayer.Select(1, 7, game, PlayerOne);

            game.CurrentPlayer = PlayerTwo;

            game.CurrentPlayer.Select(1, 0, game, PlayerTwo);
            // Assert
            PlayerOneKnight.AllowedMovement.Count().Should().Be(2);
            PlayerTwoKnight.AllowedMovement.Count().Should().Be(2);

        }
        [Fact]
        public void KnightShouldHaveEightValidMovesForPlayerOne()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();
            GamePiece? PlayerOneKnight = PlayerOne!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("B", 7)).FirstOrDefault();
            // Act
            PlayerOneKnight.CurrentPosition = ("D", 3);
            game.CurrentPlayer.Select(3, 3, game, PlayerOne);
            // Assert
            PlayerOneKnight.AllowedMovement.Count().Should().Be(8);

        }
        [Fact]
        public void KnightShouldHaveEightValidMovesForPlayerTwo()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();
            GamePiece? PlayerTwoKnight = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("B", 0)).FirstOrDefault();
            // Act
            PlayerTwoKnight.CurrentPosition = ("D", 4);
            game.CurrentPlayer = PlayerTwo;
            game.CurrentPlayer.Select(3, 4, game, PlayerTwo);
            // Assert
            PlayerTwoKnight.AllowedMovement.Count().Should().Be(8);

        }
        [Fact]
        public void KnightShouldMovePlayerOnePieceToValidPosition()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();
            GamePiece? PlayerOneKnight = PlayerOne!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("B", 7)).FirstOrDefault();

            // Act
            PlayerOne.Select(1, 7, game, PlayerOne);
            PlayerOne.MovePiece(0, 5, game);

            // Assert
            PlayerOneKnight.CurrentPosition.Should().Be(("A", 5));
            game.Board.Matrix[0, 5].Should().Be(PlayerOneKnight);
            game.Board.Matrix[1, 7].Should().BeNull();
        }
        [Fact]
        public void KnightShouldMovePlayerTwoPieceToValidPosition()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();

            GamePiece? PlayerOneKnight = PlayerOne!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("B", 7)).FirstOrDefault();

            GamePiece? PlayerTwoKnight = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("B", 0)).FirstOrDefault();


            // Act
            PlayerOne.Select(1, 7, game, PlayerOne);
            PlayerOne.MovePiece(0, 5, game);
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select(1, 0, game, PlayerTwo);
            PlayerTwo.MovePiece(0, 2, game);

            // Assert
            PlayerTwoKnight.CurrentPosition.Should().Be(("A", 2));
            game.Board.Matrix[0, 2].Should().Be(PlayerTwoKnight);
            game.Board.Matrix[1, 0].Should().BeNull();
        }
        [Fact]
        public void KnightShouldAllowKnightToAttack()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            game.PlaceInMatrix();

            // Setup PlayerOne
            GamePiece? PlayerOneKnight = PlayerOne!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("B", 7)).FirstOrDefault();

            // Setup PlayerTwo
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 1)).FirstOrDefault();

            // Act
            PlayerOne.Select(1, 7, game, PlayerOne);
            PlayerOne.MovePiece(2, 5, game);
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select(3, 1, game, PlayerTwo);
            PlayerTwo.MovePiece(3, 3, game);
            game.CurrentPlayer = PlayerOne;
            PlayerOne.Select(2, 5, game, PlayerOne);
            PlayerOne.MovePiece(3, 3, game);
            GamePiece? RemovedFromPlay = null;
            for (int i = 0; i < game.Board.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < game.Board.Matrix.GetLength(0); j++)
                {
                    if (game.Board.Matrix[i, j] == PlayerTwoPawn) RemovedFromPlay = game.Board.Matrix[i, j];
                }
            }
            // Assert
            PlayerOneKnight.CurrentPosition.Should().Be(("D", 3));
            game.Board.Matrix[3, 3].Should().Be(PlayerOneKnight);
            RemovedFromPlay.Should().BeNull();
        }
    }
    public class Bishop_Tests
    {
        [Fact]
        public void BishopShouldHaveFiveMovesForPlayerOne()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerOneBishop = PlayerOne!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 7)).FirstOrDefault();
            // Remove pawn @ (D,6) -> (D,5)
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 6)).FirstOrDefault();
            PlayerOnePawn.CurrentPosition = ("D", 5);
            game.PlaceInMatrix();

            // Act        
            game.CurrentPlayer = PlayerOne;
            PlayerOne.Select(2, 7, game, PlayerOne);

            // Assert
            PlayerOneBishop.AllowedMovement.Count().Should().Be(5);
        }
        [Fact]
        public void BishopShouldHaveFiveMovesForPlayerTwo()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerTwoBishop = PlayerTwo!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 0)).FirstOrDefault();

            // Move pawn @ (D,1) -> (D,2)
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 1)).FirstOrDefault();
            PlayerTwoPawn.CurrentPosition = ("D", 2);
            game.PlaceInMatrix();
            // Act            
            game.CurrentPlayer = PlayerTwo;

            game.CurrentPlayer.Select(2, 0, game, PlayerTwo);

            // Assert
            PlayerTwoBishop.AllowedMovement.Count().Should().Be(5);

        }
        [Fact]
        public void BishopShouldHaveSevenValidMovesForPlayerOne()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            GamePiece? PlayerOneBishop = PlayerOne!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 7)).FirstOrDefault();
            // Move pawn @ (D,6) -> (D,5)
            GamePiece? PlayerOnePawnOne = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 6)).FirstOrDefault();
            PlayerOnePawnOne.CurrentPosition = ("D", 5);
            // Remove pawn @ (B,6) -> (B,5)
            GamePiece? PlayerOnePawnTwo = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("B", 6)).FirstOrDefault();
            PlayerOnePawnTwo.CurrentPosition = ("B", 5);
            game.PlaceInMatrix();

            // Act            
            game.CurrentPlayer.Select(2, 7, game, PlayerOne);

            // Assert
            PlayerOneBishop.AllowedMovement.Count().Should().Be(7);
        }
        [Fact]
        public void BishopShouldHaveEightValidMovesForPlayerTwo()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            GamePiece? PlayerTwoBishop = PlayerTwo!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 0)).FirstOrDefault();
            // Remove pawn @ (D,1) -> (D,2)
            GamePiece? PlayerTwoPawnOne = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 1)).FirstOrDefault();
            PlayerTwoPawnOne.CurrentPosition = ("D", 2);
            // Remove pawn @ (B,1) -> (D,2)
            GamePiece? PlayerTwoPawnTwo = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("B", 1)).FirstOrDefault();
            PlayerTwoPawnTwo.CurrentPosition = ("B", 2);
            game.PlaceInMatrix();
            game.CurrentPlayer = PlayerTwo;
            // Act            
            game.CurrentPlayer.Select(2, 0, game, PlayerTwo);

            // Assert
            PlayerTwoBishop.AllowedMovement.Count().Should().Be(7);
        }
        // Unfinished
        [Fact]
        public void BishopShouldMovePlayerOnePieceToValidPosition()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            GamePiece? PlayerOneBishop = PlayerOne!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 7)).FirstOrDefault();
            // Move pawn @ (D,6) -> (D,5)
            GamePiece? PlayerOnePawnOne = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 6)).FirstOrDefault();
            PlayerOnePawnOne.CurrentPosition = ("D", 5);
            game.PlaceInMatrix();
            // Act
            game.CurrentPlayer.Select(2, 7, game, PlayerOne);
            game.CurrentPlayer.MovePiece(7, 2, game);

            // Assert
            PlayerOneBishop.CurrentPosition.Should().Be(("H", 2));
            game.Board.Matrix[7, 2].Should().Be(PlayerOneBishop);
            game.Board.Matrix[2, 7].Should().BeNull();
        }
        [Fact]
        public void BishopShouldMovePlayerTwoPieceToValidPosition()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerTwoBishop = PlayerTwo!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 0)).FirstOrDefault();
            // Move pawn @ (D,1) -> (D,2)
            GamePiece? PlayerTwoPawnOne = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 1)).FirstOrDefault();
            PlayerTwoPawnOne.CurrentPosition = ("D", 2);
            game.PlaceInMatrix();

            // Act
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select(2, 0, game, PlayerTwo);
            PlayerTwo.MovePiece(7, 5, game);

            // Assert
            PlayerTwoBishop.CurrentPosition.Should().Be(("H", 5));
            game.Board.Matrix[7, 5].Should().Be(PlayerTwoBishop);
            game.Board.Matrix[2, 0].Should().BeNull();
        }
        [Fact]
        public void BishopShouldAllowBishopToAttack()
        {
            // Arrange

            // Setup Game
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            // Setup PlayerOne
            GamePiece? PlayerOneBishop = PlayerOne!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 7)).FirstOrDefault();
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 6)).FirstOrDefault();
            PlayerOnePawn.CurrentPosition = ("D", 5);

            // Setup PlayerTwo
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("H", 1)).FirstOrDefault();
            PlayerTwoPawn.CurrentPosition = ("H", 2);
            game.PlaceInMatrix();

            // Act
            PlayerOne.Select(2, 7, game, PlayerOne);
            PlayerOne.MovePiece(7, 2, game);
            game.CurrentPlayer = PlayerTwo;

            GamePiece? RemovedFromPlay = null;
            for (int i = 0; i < game.Board.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < game.Board.Matrix.GetLength(0); j++)
                {
                    if (game.Board.Matrix[i, j] == PlayerTwoPawn) RemovedFromPlay = game.Board.Matrix[i, j];
                }
            }
            // Assert
            PlayerOneBishop.CurrentPosition.Should().Be(("H", 2));
            game.Board.Matrix[7, 2].Should().Be(PlayerOneBishop);
            RemovedFromPlay.Should().BeNull();
        }
    }
    public class Queen_Tests
    {
        [Fact]
        public void QueenShouldHaveNotHaveAnyMovesForBothPlayers()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            GamePiece? PlayerOneQueen = PlayerOne!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 7)).FirstOrDefault();
            GamePiece? PlayerTwoQueen = PlayerTwo!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 0)).FirstOrDefault();
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneQueen.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneQueen.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);

            game.CurrentPlayer = PlayerTwo;

            Enum.TryParse<ChessCoordinate>(PlayerTwoQueen.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneQueen.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);

            // Assert
            PlayerOneQueen.AllowedMovement.Count().Should().Be(0);
            PlayerTwoQueen.AllowedMovement.Count().Should().Be(0);
        }
        [Fact]
        public void QueenShouldHaveSixMovesForPlayerOne()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerOneQueen = PlayerOne!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 7)).FirstOrDefault();

            // Move pawn @ (E,6) -> (F,5)
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.CurrentPosition == ("E", 6)).FirstOrDefault();
            PlayerOnePawn.CurrentPosition = ("F", 5);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneQueen.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneQueen.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerOne);

            // Assert
            PlayerOneQueen.AllowedMovement.Count().Should().Be(6);

        }
        [Fact]
        public void QueenShouldHaveSixMovesForPlayerTwo()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerTwoQueen = PlayerTwo!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 0)).FirstOrDefault();

            // Move pawn @ (E,1) -> (F,2)
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.CurrentPosition == ("E", 1)).FirstOrDefault();
            PlayerTwoPawn.CurrentPosition = ("F", 2);
            game.PlaceInMatrix();
            game.CurrentPlayer = PlayerTwo;

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerTwoQueen.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoQueen.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);

            // Assert
            PlayerTwoQueen.AllowedMovement.Count().Should().Be(6);

        }
        [Fact]
        public void QueenShouldHave20MovesForPlayerOne()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerOneQueen = PlayerOne!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 7)).FirstOrDefault();

            // Move pawn @ (E,6) -> (F,5)
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.CurrentPosition == ("E", 6)).FirstOrDefault();
            PlayerOnePawn.CurrentPosition = ("F", 5);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneQueen.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneQueen.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerOne);
            game.CurrentPlayer.MovePiece(4, 4, game);
            game.CurrentPlayer = PlayerOne;
            game.CurrentPlayer.Select(4, 4, game, PlayerOne);
            // Assert
            PlayerOneQueen.AllowedMovement.Count().Should().Be(20);

        }
        [Fact]
        public void QueenShouldHave20MovesForPlayerTwo()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerTwoQueen = PlayerTwo!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 0)).FirstOrDefault();

            // Move pawn @ (E,1) -> (F,2)
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.CurrentPosition == ("E", 1)).FirstOrDefault();
            PlayerTwoPawn.CurrentPosition = ("F", 2);
            game.PlaceInMatrix();
            game.CurrentPlayer = PlayerTwo;

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerTwoQueen.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoQueen.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);
            game.CurrentPlayer.MovePiece(4, 3, game);
            game.CurrentPlayer = PlayerTwo;
            game.CurrentPlayer.Select(4, 3, game, PlayerTwo);
            // Assert
            PlayerTwoQueen.AllowedMovement.Count().Should().Be(20);

        }


        // Unfinished
        [Fact]
        public void QueenShouldAllowQueenToAttack()
        {
            // Arrange

            // Setup Game
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            // Setup PlayerOne
            GamePiece? PlayerOneQueen = PlayerOne!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 7)).FirstOrDefault();
            PlayerOneQueen.CurrentPosition = ("D", 5);

            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 1)).FirstOrDefault();
            game.PlaceInMatrix();
            // Act
            PlayerOne.Select(3, 5, game, PlayerOne);
            PlayerOne.MovePiece(3, 1, game);

            GamePiece? RemovedFromPlay = null;
            for (int i = 0; i < game.Board.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < game.Board.Matrix.GetLength(0); j++)
                {
                    if (game.Board.Matrix[i, j] == PlayerTwoPawn) RemovedFromPlay = game.Board.Matrix[i, j];
                }
            }
            // Assert
            PlayerOneQueen.CurrentPosition.Should().Be(("D", 1));
            game.Board.Matrix[3, 1].Should().Be(PlayerOneQueen);
            RemovedFromPlay.Should().BeNull();
        }
    }
    public class King_Tests
    {
        [Fact]
        public void KingShouldHaveNotHaveAnyMovesForBothPlayers()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();
            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();
            GamePiece? PlayerTwoKing = PlayerTwo!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 0)).FirstOrDefault();
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneKing.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneKing.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);

            game.CurrentPlayer = PlayerTwo;

            Enum.TryParse<ChessCoordinate>(PlayerTwoKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneKing.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);

            // Assert
            PlayerOneKing.AllowedMovement.Count().Should().Be(0);
            PlayerTwoKing.AllowedMovement.Count().Should().Be(0);
        }
        [Fact]
        public void KingShouldHaveOneMove()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();

            // Move pawn @ (E,6) -> (F,5)
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.CurrentPosition == ("D", 6)).FirstOrDefault();
            PlayerOnePawn.CurrentPosition = ("D", 5);

            GamePiece? PlayerTwoKing = PlayerTwo!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 0)).FirstOrDefault();

            // Move pawn @ (E,6) -> (F,5)
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.CurrentPosition == ("D", 1)).FirstOrDefault();
            PlayerTwoPawn.CurrentPosition = ("D", 2);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneKing.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneKing.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);

            game.CurrentPlayer = PlayerTwo;

            Enum.TryParse<ChessCoordinate>(PlayerTwoKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoKing.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);

            // Assert
            PlayerOneKing.AllowedMovement.Count().Should().Be(1);
            PlayerTwoKing.AllowedMovement.Count().Should().Be(1);

        }
        [Fact]
        public void KingShouldHaveEightMovesForPlayerOne()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.FirstOrDefault(p => p.Name == "King" && p.CurrentPosition == ("D", 7));
            // Place King @ (D,4) 
            PlayerOneKing.CurrentPosition = ("D", 5);

            // Move Pawn @ (C,6)
            GamePiece? PlayerOnePawn01 = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("C", 6)).FirstOrDefault();
            PlayerOnePawn01.CurrentPosition = ("A", 5);
            // Move Pawn @ (D,6)
            GamePiece? PlayerOnePawn02 = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 6)).FirstOrDefault();
            PlayerOnePawn02.CurrentPosition = ("A", 4);
            // Move Pawn @ (E,6)
            GamePiece? PlayerOnePawn03 = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("E", 6)).FirstOrDefault();
            PlayerOnePawn03.CurrentPosition = ("A", 3);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneKing.CurrentPosition.Y;
            game.CurrentPlayer.Select(3, 5, game, PlayerOne);
            // Assert
            PlayerOneKing.AllowedMovement.Count().Should().Be(8);

        }
        [Fact]
        public void KingShouldHaveEightMovesForPlayerTwo()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerTwoKing = PlayerTwo!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 0)).FirstOrDefault();

            // Place King @ (D,3) 
            PlayerTwoKing.CurrentPosition = ("D", 2);
            // Move Pawn @ (C,1)
            GamePiece? PlayerTwoPawn01 = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("C", 1)).FirstOrDefault();
            PlayerTwoPawn01.CurrentPosition = ("A", 5);
            // Move Pawn @ (D,1)
            GamePiece? PlayerTwoPawn02 = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 1)).FirstOrDefault();
            PlayerTwoPawn02.CurrentPosition = ("A", 4);
            // Move Pawn @ (E,1)
            GamePiece? PlayerTwoPawn03 = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("E", 1)).FirstOrDefault();
            PlayerTwoPawn03.CurrentPosition = ("A", 3);
            game.PlaceInMatrix();

            game.CurrentPlayer = PlayerTwo;

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerTwoKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoKing.CurrentPosition.Y;
            game.CurrentPlayer.Select(3, 2, game, PlayerTwo);
            // Assert
            PlayerTwoKing.AllowedMovement.Count().Should().Be(8);
        }
        [Fact]
        public void KingShouldHaveFiveMovesForPlayerOne()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();
            // Place King @ (D,4) 
            PlayerOneKing.CurrentPosition = ("D", 4);


            GamePiece? PlayerTwoRook = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("A", 0)).FirstOrDefault();
            PlayerTwoRook.CurrentPosition = ("A", 3);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneKing.CurrentPosition.Y;
            game.CurrentPlayer.Select(3, 4, game, PlayerOne);
            // Assert
            PlayerOneKing.AllowedMovement.Count().Should().Be(5);

        }
        [Fact]
        public void KingShouldHaveFiveMovesForPlayerTwo()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerTwoKing = PlayerTwo!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 0)).FirstOrDefault();
            // Place King @ (D,4) 
            PlayerTwoKing.CurrentPosition = ("D", 3);


            GamePiece? PlayerOneRook = PlayerOne!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("A", 7)).FirstOrDefault();
            PlayerOneRook.CurrentPosition = ("A", 4);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerTwoKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoKing.CurrentPosition.Y;

            game.CurrentPlayer = PlayerTwo;
            game.CurrentPlayer.Select(3, 3, game, PlayerTwo);
            // Assert
            PlayerTwoKing.AllowedMovement.Count().Should().Be(5);
        }
        [Fact]
        public void ShouldPutPlayerOneInCheck()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();
            // Place King @ (H,5) 
            PlayerOneKing.CurrentPosition = ("H", 4);

            GamePiece? PlayerTwoKnight = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("B", 0)).FirstOrDefault();
            // Place Knight @ (D,2) 
            PlayerTwoKnight.CurrentPosition = ("D", 2);
            // PlayerOneKing.CurrentPosition = ("G", 3);
            game.PlaceInMatrix();
            // Act
            game.CurrentPlayer = PlayerTwo;

            game.CurrentPlayer.Select(3, 2, game, PlayerTwo);
            game.CurrentPlayer.MovePiece(5, 3, game);
            game.NewTurn();

            // Assert
            PlayerOne.Check.Should().BeTrue();
            PlayerOne.SelectedPiece.Should().Be(PlayerOneKing);
        }

        [Fact]
        public void KingShouldHaveNotHaveMovesAndTriggerGameOver()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();
            // Place King @ (D,4) 
            PlayerOneKing.CurrentPosition = ("D", 3);


            GamePiece? PlayerTwoRookOne = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("A", 0)).FirstOrDefault();
            PlayerTwoRookOne.CurrentPosition = ("A", 2);
            GamePiece? PlayerTwoRookTwo = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("A", 0)).FirstOrDefault();
            PlayerTwoRookOne.CurrentPosition = ("A", 4);
            GamePiece? PlayerTwoKnightOne = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("B", 0)).FirstOrDefault();
            PlayerTwoKnightOne.CurrentPosition = ("B", 5);
            GamePiece? PlayerTwoKnightTwo = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("G", 0)).FirstOrDefault();
            PlayerTwoKnightTwo.CurrentPosition = ("F", 5);
            GamePiece? PlayerTwoQueen = PlayerTwo!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 0)).FirstOrDefault();
            PlayerTwoQueen.CurrentPosition = ("D", 5);
            game.PlaceInMatrix();

            // Act            
            game.CurrentPlayer = PlayerTwo;
            game.CurrentPlayer.Select(1, 5, game, PlayerTwo);
            game.NewTurn();
            // game.CurrentPlayer.Select(3, 3, game, PlayerOne);
            // Assert
            PlayerOneKing.AllowedMovement.Count.Should().Be(0);
            game.GameOver.Should().BeTrue();
            game.Winner.Should().Be(PlayerTwo);
            PlayerOne.Losses.Should().Be(1);
            PlayerTwo.Wins.Should().Be(1);
        }
        [Fact]
        public void KingCanNotPutItselfInDanger()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();
            // Place King @ (D,4) 
            PlayerOneKing.CurrentPosition = ("D", 3);


            GamePiece? PlayerTwoRookOne = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("A", 0)).FirstOrDefault();
            PlayerTwoRookOne.CurrentPosition = ("A", 2);
            GamePiece? PlayerTwoRookTwo = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("A", 0)).FirstOrDefault();
            PlayerTwoRookOne.CurrentPosition = ("A", 4);
            GamePiece? PlayerTwoKnightOne = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("B", 0)).FirstOrDefault();
            PlayerTwoKnightOne.CurrentPosition = ("B", 5);
            GamePiece? PlayerTwoKnightTwo = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("G", 0)).FirstOrDefault();
            PlayerTwoKnightTwo.CurrentPosition = ("F", 5);
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("E", 1)).FirstOrDefault();
            PlayerTwoPawn.CurrentPosition = ("E", 2);
            game.PlaceInMatrix();

            // Act            
            game.CurrentPlayer = PlayerTwo;
            game.CurrentPlayer.Select(1, 5, game, PlayerTwo);
            game.NewTurn();
            // game.CurrentPlayer.Select(3, 3, game, PlayerOne);
            // Assert
            PlayerOneKing.AllowedMovement.Count.Should().Be(0);
            game.GameOver.Should().BeTrue();
            PlayerOne.Losses.Should().Be(1);
            PlayerTwo.Wins.Should().Be(1);
        }
        [Fact]
        public void KingCanNotMoveWherePlayerTwoPawnCanAttack()
        {
            // Arrange
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 6)).FirstOrDefault();
            PlayerOnePawn.CurrentPosition = ("A", 5);

            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();
            PlayerOneKing.CurrentPosition = ("D", 6);

            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn" && p.CurrentPosition == ("D", 1)).FirstOrDefault();
            PlayerTwoPawn.CurrentPosition = ("D", 4);

            game.PlaceInMatrix();

            // Act            
            game.CurrentPlayer.Select(3, 6, game, PlayerOne);
            // Assert
            // PlayerTwoKing.AllowedMovement.Count.Should().Be(2);
            PlayerOneKing.AllowedMovement.Should().NotContain((2, 5));
            PlayerOneKing.AllowedMovement.Should().NotContain((4, 5));
            PlayerOneKing.AllowedMovement[0].Should().Be((3, 5));
            PlayerOneKing.AllowedMovement[1].Should().Be((3, 7));

        }

        [Fact]
        public void KingShouldAllowKingToAttack()
        {
            // Arrange

            // Setup Game
            (var PlayerOne, var PlayerTwo, var game) = Test_Setup.CreateDefaultGame();

            // Setup PlayerOne
            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();
            PlayerOneKing.CurrentPosition = ("D", 4);
            (int P1x, int P1Y) = Chess.ParsePosition(PlayerOneKing.CurrentPosition);

            // Setup PlayerTwo
            GamePiece? PlayerTwoBishop = PlayerTwo!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 0)).FirstOrDefault();
            PlayerTwoBishop.CurrentPosition = ("E", 4);
            (int P2x, int P2Y) = Chess.ParsePosition(PlayerTwoBishop.CurrentPosition);
            game.PlaceInMatrix();

            // Act
            PlayerOne.Select(3, 4, game, PlayerOne);
            PlayerOne.MovePiece(4, 4, game);

            GamePiece? RemovedFromPlay = null;
            for (int i = 0; i < game.Board.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < game.Board.Matrix.GetLength(1); j++)
                {
                    if (game.Board.Matrix[i, j] == PlayerTwoBishop) RemovedFromPlay = game.Board.Matrix[i, j];
                }
            }
            // Assert
            PlayerOneKing.CurrentPosition.Should().Be(("E", 4));
            game.Board.Matrix[4, 4].Should().Be(PlayerOneKing);
            RemovedFromPlay.Should().BeNull();
        }
    }

}

public class GameBoard_Tests
{
    [Fact]
    public void ChessBoardShouldBe8X8()
    {
        // Arrange
        // Act
        GameBoard gameBoard = new GameBoard(GameType.Chess);

        // Assert
        gameBoard.BoardSize.Should().Be((8, 8));
    }
}

public class ChessGame_Tests
{
    // Requires path: "FilePaths.Tests" in LayoutGamePieces 
    [Fact]
    public void PlayerOneShouldBeAbleToJoinGame()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        chess.JoinGame(playerOne, JoinAs.Player);
        // Assert
        chess.PlayerOne.Username.Should().Be("John");
    }
    // Requires path: "FilePaths.Tests" in LayoutGamePieces 
    [Fact]
    public void PlayerTwoShouldBeAbleToJoinGame()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        Player playerTwo = new Player("Jane");
        chess.JoinGame(playerOne, JoinAs.Player);
        chess.JoinGame(playerTwo, JoinAs.Player);
        // Assert
        chess.PlayerOne.Username.Should().Be("John");
        chess.PlayerTwo.Username.Should().Be("Jane");
    }

    // Requires path: "FilePaths.Tests" in LayoutGamePieces 
    [Fact]
    public void ShouldAddSpectatorIfPlayerSeatsAreFull()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        Player playerTwo = new Player("Jane");
        Player SpectatorOne = new Player("Jack");
        chess.JoinGame(playerOne, JoinAs.Player);
        chess.JoinGame(playerTwo, JoinAs.Player);
        chess.JoinGame(SpectatorOne, JoinAs.Spectator);
        // Assert
        chess.Spectators[0].Should().Be(SpectatorOne);

    }
    // Requires path: "FilePaths.Tests" in LayoutGamePieces 
    [Fact]
    public void PlayerOnePieceShouldHaveCorrectPosition()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        chess.JoinGame(playerOne, JoinAs.Player);
        // Assert
        chess.PlayerOne.GamePieces[1].CurrentPosition.Should().Be((ChessCoordinate.B.ToString(), 6));
    }

    // Requires path: "FilePaths.Tests" in LayoutGamePieces 
    [Fact]
    public void PlayerTwoPieceShouldHaveCorrectPosition()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        Player playerTwo = new Player("Jane");
        chess.JoinGame(playerOne, JoinAs.Player);
        chess.JoinGame(playerTwo, JoinAs.Player);

        // Assert
        chess.PlayerTwo.GamePieces[0].CurrentPosition.Should().Be((ChessCoordinate.A.ToString(), 1));
    }

}

public class DataManager_Tests
{
    [Fact]
    public void LoadFileShouldNotThrowDataLoadErrorException()
    {
        // Arrange
        void Load()
        {
            DataManager.LoadFile<PlayerData>("path.json");
        }
        // Act
        FluentActions.Invoking(Load).Should().Throw<DataLoadErrorException>();
        // Assert
    }
}

public class Lobby_Tests
{
    [Fact]
    public void ShouldHaveAListOfGamesForEachGameType()
    {
        // Arrange

        // Act

        // Assert

    }
    [Fact]
    public void ShouldFilterActiveGamesWithCorrectGuid()
    {
        // Arrange

        // Act

        // Assert

    }
    [Fact]
    public void ShouldFilterOpenGamesWithCorrectGuid()
    {
        // Arrange
        Player playerOne = new("P1");
        Player playerTwo = new("P2");
        Player playerThree = new("P3");
        int previousGameCount = LobbyManager.ChessGames.Select(d => d.Value.Open == true).ToList().Count();
        Guid g1 = LobbyManager.CreateGame(GameType.Chess, playerOne);
        Guid g2 = LobbyManager.CreateGame(GameType.Chess, playerThree);
        Game gameOne = LobbyManager.GetGame(GameType.Chess, g1);
        Game gameTwo = LobbyManager.GetGame(GameType.Chess, g2);
        gameOne.JoinGame(playerTwo, JoinAs.Player);
        // Act
        var OpenGames = LobbyManager.FilterByOpen(GameType.Chess);

        // Assert
        OpenGames.ContainsKey(gameTwo.UUID).Should().BeTrue();
        OpenGames.Count().Should().Be(previousGameCount + 1);
    }
    [Fact]
    public void ShouldAddANewGameToLobby()
    {
        // Arrange

        Player player = new Player("John");
        Guid newGameGuid = LobbyManager.CreateGame(GameType.Chess, player);
        // Act
        // Assert
        LobbyManager.ChessGames.FirstOrDefault(g => g.Key == newGameGuid).Should().NotBeNull();
    }
}

public class DB_Tests
{
    [Fact]
    public void ShouldInsertANewRecordAndReturnTrue()
    {
        // Arrange
        DB_Connect dB_Connect = new();
        string username = "Test";
        string password = "SooperSecret";
        // Act
        Player TestPlayer = new(username);
        bool result = dB_Connect.InsertRecord(TestPlayer, password);
        // Assert
        result.Should().BeTrue();
    }
    [Fact]
    public void ShouldReturnTrueIfUserExists()
    {
        // Arrange
        DB_Connect dB_Connect = new();
        // Act
        bool UserExists = dB_Connect.AccountExists("Test");
        // Assert
        UserExists.Should().BeTrue();
    }
    [Fact]
    public void ShouldDeleteAnExistingRecord()
    {
        // Arrange
        DB_Connect dB_Connect = new();
        string username = "Test";
        // Act
        dB_Connect.DeleteRecord(username);
        bool StillExists = dB_Connect.AccountExists(username);
        // Assert
        StillExists.Should().BeFalse();
    }
    [Fact]
    public void ShouldReturnFalseIfUserExists()
    {
        // Arrange
        DB_Connect dB_Connect = new();
        // Act
        bool UserExists = dB_Connect.AccountExists("Test1");
        // Assert
        UserExists.Should().BeFalse();
    }

}

public class Login_Tests
{
    [Fact]
    public void ShouldCreateAnAccountIfUsernameIsNotInUse()
    {
        // Arrange
        string username = "username";
        string password = "soopersecure";
        string confirm = "soopersecure";
        DB_Connect dB_Connect = new();
        if (dB_Connect.AccountExists(username))
        {
            dB_Connect.DeleteRecord(username);
        }
        // Act
        Player player = PlayerManager.SignUp(username, password, confirm);
        // Assert
        player.Username.Should().Be(username);
    }
    [Fact]
    public void ShouldSignIntoAccountIfUsernameAndPasswordCombinationMatch()
    {
        // Arrange
        string username = "username";
        string password = "soopersecure";
        // Act
        var result = PlayerManager.SignIn(username, password);
        // Assert
        result.Should().NotBeNull();
    }
    [Fact]
    public void ShouldAddPlayerToListOfOnlinePlayers()
    {
        // Arrange
        string username = "username";
        string password = "soopersecure";
        // Act
        var result = PlayerManager.SignIn(username, password);
        // Assert
        result.Should().NotBeNull();
    }

}