﻿namespace CureAutoGold.Infra.Models.Server;

public class Gamedbd : IPwDaemonConfig
{
    public string Host { get; }
    public int Port { get; }

    public Gamedbd(string host, int port)
    {
        Host = host;
        Port = port;
    }
}