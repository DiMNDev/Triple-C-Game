namespace Chess_Final.Tests;
using Player;
using FluentAssertions;
using Chess_Final.Generics;
using Chess_Final.Chess;
using Lobby;
using TC_DataManagerException;
using TC_DataManager;
using Chess_Final.PlayerManager;

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
        chess.PlayerOne.Username.Should().Be("John");
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
        chess.PlayerOne.Username.Should().Be("John");
        chess.PlayerTwo.Username.Should().Be("Jane");
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
        Lobby lobby = new Lobby();
        Player playerOne = new("P1");
        Player playerTwo = new("P2");
        Player playerThree = new("P3");
        Guid g1 = lobby.CreateGame(playerOne, GameType.Chess);
        Guid g2 = lobby.CreateGame(playerTwo, GameType.Chess);
        Game gameOne = Lobby.Instance.GetGame(GameType.Chess, g1);
        Game gameTwo = Lobby.Instance.GetGame(GameType.Chess, g2);
        gameTwo.JoinGame(playerThree);
        // Act
        var OpenGames = lobby.FilterByOpen(GameType.Chess);
        // Assert
        OpenGames.Contains((false, true, gameOne)).Should().BeTrue();
        OpenGames.Count().Should().Be(1);
    }
    [Fact]
    public void ShouldAddANewGameToLobby()
    {
        // Arrange
        Lobby lobby = new Lobby();
        Player player = new Player("John");
        Guid newGameGuid = lobby.CreateGame(player, GameType.Chess);
        // Act
        // Assert
        lobby.ChessGames.FirstOrDefault(g => g.Key == newGameGuid).Should().NotBeNull();
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
        // Act
        var result = PlayerManager.SignUp(username, password, confirm);
        // Assert
        result.Should().NotBeNull();
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

public class DB_Tests
{
    [Fact]
    public void ShouldInsertANewRecord()
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