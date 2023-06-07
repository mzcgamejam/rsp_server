public class Infos
{
    public DB DB = new DB();
    public GameLift GameLift = new GameLift();
}

public class DB
{
    public string IP;
    public uint Port;
    public string User;
    public string Password;
    public string Charset;
    public string Database;
}

public class GameLift
{
    public string FleetId;
    public string ServiceURL;
}
