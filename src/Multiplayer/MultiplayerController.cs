using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Godot;
using Godot.Collections;

namespace OutofTheHole.Multiplayer;


public partial class MultiplayerController : CanvasLayer
{
	[Export] private string _address = "127.0.0.1";
	//[Export] private PackedScene _fallbackScene;

	private ENetMultiplayerPeer _peer;

	[Export] private int _port = 4242;

	private int _currentAddrIdx = 0;

	private List<string> _addrArray = new List<string>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<LineEdit>("MenuMarginContainer/MenuVBoxContainer/ButtonContainer/Username").GrabFocus();
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
		// GD.Print($"Local IP: {IP.ResolveHostname("bastien")}, {IP.GetLocalInterfaces()[0]["addresses"].AsStringArray()[0]}");

		foreach (var localInterface in IP.GetLocalInterfaces())
		{
			string[] addrList = localInterface["addresses"].AsStringArray();
			foreach (var ip in addrList)	
			{
				if (!(ip.Contains(':')))
				{
					_addrArray.Add(ip);
				}
			}
		}
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
				"MenuMarginContainer/MenuVBoxContainer/ButtonContainer/StartButton")
			.Disabled = false;
		GetNode<Button>(
			"MenuMarginContainer/MenuVBoxContainer/ButtonContainer/StartButton").Show();
		GetNode<Button>("MenuMarginContainer/MenuVBoxContainer/ButtonContainer/JoinButton")
			.Hide();
		GetNode<Button>("MenuMarginContainer/MenuVBoxContainer/ButtonContainer/HostButton")
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

		Button ipAddrButton = GetNode<Button>("IPAddrButton");
		//ipAddrButton.Text = $"IP ADDRESS: {IP.GetLocalInterfaces()[_currentAddrIdx]["addresses"].AsStringArray()[0]}";
		GD.Print(_addrArray);		
		
		ipAddrButton.Text = $"IP ADDRESS: {_addrArray[0]}";
		ipAddrButton.Show();

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

	private void _on_ip_addr_button_pressed()
	{
		Button ipAddrButton = GetNode<Button>("IPAddrButton");
		_currentAddrIdx++;
		_currentAddrIdx %= _addrArray.Count();
		
		
		ipAddrButton.Text = $"IP ADDRESS: {_addrArray[_currentAddrIdx]}";
	}
	
	private void _on_return_button_button_down()
	{
		GetTree().ChangeSceneToFile("res://src/Menus/MainMenu.tscn");
	}

	/// <summary>
	/// Function that gets the local IP address
	/// </summary>
	/// <returns>
	/// The local IP address
	/// </returns>
	private static string _get_ip()
	{
		var host = Dns.GetHostEntry(Dns.GetHostName());
		GD.Print("-- DEBUG ALL IP --");
		foreach (var ip in host.AddressList)
		{
			GD.Print(ip);
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				return ip.ToString();
			}
		}
		throw new Exception("No network adapters with an IPv4 address in the system!");
	}
}
