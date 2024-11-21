namespace Chess_Final.Tests;
using Player;
using FluentAssertions;
using Chess_Final.Generics;
using Chess_Final.Chess;
using TC_DataManagerException;
using TC_DataManager;

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
        player.Name.Should().Be(newPlayerName);
    }

}

public class GamePiece_Tests
{
    [Fact]
    public void ShouldHaveValidMove()
    {
        // Arrange

        // Act

        // Assert

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
    [Fact]
    public void PlayerOneShouldBeAbleToJoinGame()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        chess.JoinGame(playerOne);
        // Assert
        chess.PlayerOne.Name.Should().Be("John");
    }
    [Fact]
    public void PlayerTwoShouldBeAbleToJoinGame()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        Player playerTwo = new Player("Jane");
        chess.JoinGame(playerOne);
        chess.JoinGame(playerTwo);
        // Assert
        chess.PlayerOne.Name.Should().Be("John");
        chess.PlayerTwo.Name.Should().Be("Jane");
    }

    [Fact]
    public void ShouldAddSpectatorIfPlayerSeatsAreFull()
    {

        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        Player playerTwo = new Player("Jane");
        Player SpectatorOne = new Player("Jack");
        chess.JoinGame(playerOne);
        chess.JoinGame(playerTwo);
        chess.JoinGame(SpectatorOne);
        // Assert
        chess.Spectators[0].Should().Be(SpectatorOne);

    }

    [Fact]
    public void PlayerOnePieceShouldHaveCorrectPosition()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        chess.JoinGame(playerOne);
        // Assert
        chess.PlayerOne.GamePieces[1].CurrentPosition.Should().Be((ChessCoordinate.B.ToString(), 1));
    }

    [Fact]
    public void PlayerTwoPieceShouldHaveCorrectPosition()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        Player playerTwo = new Player("Jane");
        chess.JoinGame(playerOne);
        chess.JoinGame(playerTwo);

        // Assert
        chess.PlayerTwo.GamePieces[0].CurrentPosition.Should().Be(("A", 6));
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

        // Act

        // Assert

    }
    [Fact]
    public void ShouldAddANewGameToLobby()
    {
        // Arrange
        Player player = new Player("John");
         Game newGame = Lobby.Lobby.CreateGame(player, GameType.Chess);
        // Act
       Guid gameGuid = newGame.UUID;
        // Assert
        

    }
}