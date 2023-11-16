using Godot;
using System;

public partial class MultiplayerController : Control
{
	[Export]
	private int port = 4242;
	[Export]
	private string address = "127.0.0.1";

	private ENetMultiplayerPeer peer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
	}

	/// <summary>
	///  Runs when the connection fails and it runs only on the client
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
	}

	/// <summary>
	/// Runs when a player disconnects and runs on all peers
	/// </summary>
	/// <param name="id">id of the player that disconnect</param>
	/// <exception cref="NotImplementedException"></exception>
	private void PeerDisconnected(long id)
	{
		GD.Print("Player disconnected: " + id.ToString());
	}

	/// <summary>
	/// Runs when a player connects and run on all peers
	/// </summary>
	/// <param name="id">id of the player that connected</param>
	/// <exception cref="NotImplementedException"></exception>
	private void PeerConnected(long id)
	{
		GD.Print("Player connected: " + id.ToString());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	/// <summary>
	/// Function called when the host button is pressed
	/// </summary>
	private void _on_host_button_button_down()
	{
		peer = new ENetMultiplayerPeer();
		var error = peer.CreateServer(port, 2);
		if (error != Error.Ok)
		{
			GD.Print("error cannot host!" + error.ToString());
			return;
		}
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Waiting for Players!");
	}
	
	/// <summary>
	/// Function called when the join button is pressed
	/// </summary>
	private void _on_join_button_button_down()
	{
		peer = new ENetMultiplayerPeer();
		peer.CreateClient(address, port);
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Joining Game!");
	}


	/// <summary>
	/// Function called when the start game button is pressed
	/// </summary>
	private void _on_start_game_button_button_down()
	{
		Rpc("startGame");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void startGame()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://test_scene.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		this.Hide();
	}
}



