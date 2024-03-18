using System;
using Godot;

namespace OutofTheHole.Multiplayer;


public partial class MultiplayerController : CanvasLayer
{
	[Export] private string _address = "127.0.0.1";
	//[Export] private PackedScene _fallbackScene;

	private ENetMultiplayerPeer _peer;

	[Export] private int _port = 4242;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<LineEdit>("MenuMarginContainer/MenuVBoxContainer/ButtonContainer/Username").GrabFocus();
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
	}

	private void QueryAdress()
	{
		var queryField = GetNode<LineEdit>("MenuMarginContainer/MenuVBoxContainer/ButtonContainer/RemoteAddress");
		GetNode<LineEdit>("MenuMarginContainer/MenuVBoxContainer/ButtonContainer/RemoteAddress").Show();
	}

	/// <summary>
	/// Runs when the connection fails and it runs only on the client
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	private void ConnectionFailed()
	{
		GD.Print("CONNECTION FAILED");
	}

	/// <summary>
	/// Runs when the connection is successful and only runs on the client
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	private void ConnectedToServer()
	{
		GD.Print("Connected to server");
		RpcId(1, "SendPlayerInformation",
			GetNode<LineEdit>("MenuMarginContainer/MenuVBoxContainer/ButtonContainer/Username").Text,
			Multiplayer.GetUniqueId());
	}

	/// <summary>
	/// Runs when a player disconnects and runs on all peers
	/// </summary>
	/// <param name="id">id of the player that disconnect</param>
	/// <exception cref="NotImplementedException"></exception>
	private void PeerDisconnected(long id)
	{
		GD.Print("PlayerInfo disconnected: " + id);
		GetTree().ChangeSceneToFile("res://src/Menus/MainMenu.tscn");

	}

	/// <summary>
	/// Runs when a player connects and run on all peers
	/// </summary>
	/// <param name="id">id of the player that connected</param>
	/// <exception cref="NotImplementedException"></exception>
	private void PeerConnected(long id)
	{
		GD.Print("PlayerInfo connected: " + id);
		GetNode<Button>(
				"MenuMarginContainer/MenuVBoxContainer/ButtonContainer/StartButtonMarginContainer/StartGameButton")
			.Disabled = false;
		GetNode<Button>(
			"MenuMarginContainer/MenuVBoxContainer/ButtonContainer/StartButtonMarginContainer/StartGameButton").Show();
		GetNode<Button>("MenuMarginContainer/MenuVBoxContainer/ButtonContainer/JoinButtonMarginContainer/JoinButton")
			.Hide();
		GetNode<Button>("MenuMarginContainer/MenuVBoxContainer/ButtonContainer/HostButtonMarginContainer/HostButton")
			.Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// Function called when the host button is pressed
	/// </summary>
	private void _on_host_button_down()
	{
		_peer = new ENetMultiplayerPeer();
		var error = _peer.CreateServer(_port, 2);
		if (error != Error.Ok)
		{
			GD.Print("error cannot host!" + error);
			return;
		}

		var joinButton =
			GetNode<Button>(
				"MenuMarginContainer/MenuVBoxContainer/ButtonContainer/JoinButton");
		joinButton.Hide();
		GetNode<Button>(
				"MenuMarginContainer/MenuVBoxContainer/ButtonContainer/ConnectButton")
			.Disabled =
			true;
		GetNode<Button>(
			"MenuMarginContainer/MenuVBoxContainer/ButtonContainer/ConnectButton").Show();

		_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Waiting for Players!");
		SendPlayerInformation(GetNode<LineEdit>("MenuMarginContainer/MenuVBoxContainer/ButtonContainer/Username").Text,
			1);
	}

	/// <summary>
	/// Function called when the join button is pressed
	/// </summary>
	private void _on_join_button_down()
	{
		QueryAdress();

		var hostButton =
			GetNode<Button>(
				"MenuMarginContainer/MenuVBoxContainer/ButtonContainer/HostButton");
		var joinButton =
			GetNode<Button>(
				"MenuMarginContainer/MenuVBoxContainer/ButtonContainer/JoinButton");

		hostButton.Hide();

		//joinButton.SetPosition(new Vector2(joinButton.Position.X - 210, joinButton.Position.Y));
		//joinButton.Text = "Start";
		joinButton.Hide();
		GetNode<Button>(
			"MenuMarginContainer/MenuVBoxContainer/ButtonContainer/ConnectButton").Show();
	}

	private void _on_connect_button_down()
	{
		var addressInput = GetNode<LineEdit>("MenuMarginContainer/MenuVBoxContainer/ButtonContainer/RemoteAddress")
			.Text;
		if (addressInput != "") _address = addressInput;
		_peer = new ENetMultiplayerPeer();
		_peer.CreateClient(_address, _port);
		_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Joining Game!");
		GD.Print(_address);

		GetNode<Button>(
			"MenuMarginContainer/MenuVBoxContainer/ButtonContainer/ConnectButton").Hide();
		GetNode<Button>(
			"MenuMarginContainer/MenuVBoxContainer/ButtonContainer/StartGameButton").Show();

		//GetNode<Button>("ConnectButton").Connect("button_down", Callable.From(_on_start_game_button_button_down));
	}


	/// <summary>
	/// Function called when the start game button is pressed
	/// </summary>
	private void _on_start_button_down()
	{
		Rpc("StartGame");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartGame()
	{
		foreach (var item in GameManager.Players) GD.Print(item.Name + " is playing");
		var scene = ResourceLoader.Load<PackedScene>("res://src/Map/Map.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		Hide();
	}

	/// <summary>
	/// Send player information to all client
	/// </summary>
	/// <param name="name">The name of the player</param>
	/// <param name="id">The id of the player</param>
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SendPlayerInformation(string name, int id)
	{
		int playerRole;
		if (id == 1)
			playerRole = 1;
		else
			playerRole = 2;
		var playerInfo = new PlayerInfo
		{
			Name = name,
			Id = id,
			Role = playerRole
		};
		if (!GameManager.Players.Contains(playerInfo)) GameManager.Players.Add(playerInfo);

		if (Multiplayer.IsServer())
			foreach (var item in GameManager.Players)
				Rpc("SendPlayerInformation", item.Name, item.Id);
	}

	private void _on_return_button_button_down()
	{
		GetTree().ChangeSceneToFile("res://src/Menus/MainMenu.tscn");
	}
}
