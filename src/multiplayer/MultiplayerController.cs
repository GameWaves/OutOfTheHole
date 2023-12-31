using System;
using Godot;
using OutofTheHole.multiplayer;

public partial class MultiplayerController : Control
{
	[Export] private string address = "127.0.0.1";

	private ENetMultiplayerPeer peer;

	[Export] private int port = 4242;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
	}

	private void QueryAdress()
	{
		var queryField = GetNode<LineEdit>("RemoteAddress");
		GetNode<LineEdit>("RemoteAddress").Show();
	}

	/// <summary>
	///     Runs when the connection fails and it runs only on the client
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	private void ConnectionFailed()
	{
		GD.Print("CONNECTION FAILED");
	}

	/// <summary>
	///     Runs when the connection is successful and only runs on the client
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	private void ConnectedToServer()
	{
		GD.Print("Connected to server");
		RpcId(1, "sendPlayerInformation", GetNode<LineEdit>("Username").Text, Multiplayer.GetUniqueId());
	}

	/// <summary>
	///     Runs when a player disconnects and runs on all peers
	/// </summary>
	/// <param name="id">id of the player that disconnect</param>
	/// <exception cref="NotImplementedException"></exception>
	private void PeerDisconnected(long id)
	{
		GD.Print("PlayerInfo disconnected: " + id);
	}

	/// <summary>
	///     Runs when a player connects and run on all peers
	/// </summary>
	/// <param name="id">id of the player that connected</param>
	/// <exception cref="NotImplementedException"></exception>
	private void PeerConnected(long id)
	{
		GD.Print("PlayerInfo connected: " + id);
		GetNode<Button>("StartGameButton").Show();
		GetNode<Button>("JoinButton").Hide();
		GetNode<Button>("HostButton").Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	///     Function called when the host button is pressed
	/// </summary>
	private void _on_host_button_button_down()
	{
		peer = new ENetMultiplayerPeer();
		var error = peer.CreateServer(port, 2);
		if (error != Error.Ok)
		{
			GD.Print("error cannot host!" + error);
			return;
		}

		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Waiting for Players!");
		sendPlayerInformation(GetNode<LineEdit>("Username").Text, 1);
	}

	/// <summary>
	///     Function called when the join button is pressed
	/// </summary>
	private void _on_join_button_button_down()
	{
		QueryAdress();

		var hostButton = GetNode<Button>("HostButton");
		var joinButton = GetNode<Button>("JoinButton");

		hostButton.Hide();

		//joinButton.SetPosition(new Vector2(joinButton.Position.X - 210, joinButton.Position.Y));
		//joinButton.Text = "Start";
		joinButton.Hide();
		GetNode<Button>("StartButton").Show();
	}

	private void _on_start_button_button_down()
	{
		var addressInput = GetNode<LineEdit>("RemoteAddress").Text;
		if (addressInput != "") address = addressInput;
		peer = new ENetMultiplayerPeer();
		peer.CreateClient(address, port);
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Joining Game!");
		GD.Print(address);

		GetNode<Button>("StartButton").Hide();
		GetNode<Button>("StartGameButton").Show();

		//GetNode<Button>("StartButton").Connect("button_down", Callable.From(_on_start_game_button_button_down));
	}


	/// <summary>
	///     Function called when the start game button is pressed
	/// </summary>
	private void _on_start_game_button_button_down()
	{
		Rpc("startGame");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void startGame()
	{
		foreach (var item in GameManager.Players) GD.Print(item.Name + " is playing");
		var scene = ResourceLoader.Load<PackedScene>("res://src/map/Map.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		Hide();
	}

	/// <summary>
	///     Send player information to all client
	/// </summary>
	/// <param name="name">The name of the player</param>
	/// <param name="id">The id of the player</param>
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void sendPlayerInformation(string name, int id)
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
				Rpc("sendPlayerInformation", item.Name, item.Id);
	}
}
