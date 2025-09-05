namespace Godot.multiplayer;

public partial class SteamworksPeer : MultiplayerPeerExtension
{
    /// <summary>
    /// <para>Called when the multiplayer peer should be immediately closed (see <see cref="Godot.MultiplayerPeer.Close()"/>).</para>
    /// </summary>
    public override void _Close()
    {
    }

    /// <summary>
    /// <para>Called when the connected <paramref name="pPeer"/> should be forcibly disconnected (see <see cref="Godot.MultiplayerPeer.DisconnectPeer(int, bool)"/>).</para>
    /// </summary>
    public override void _DisconnectPeer(int pPeer, bool pForce)
    {
    }

    /// <summary>
    /// <para>Called when the available packet count is internally requested by the <see cref="Godot.MultiplayerApi"/>.</para>
    /// </summary>
    public override int _GetAvailablePacketCount()
    {
        return default;
    }

    /// <summary>
    /// <para>Called when the connection status is requested on the <see cref="Godot.MultiplayerPeer"/> (see <see cref="Godot.MultiplayerPeer.GetConnectionStatus()"/>).</para>
    /// </summary>
    public override MultiplayerPeer.ConnectionStatus _GetConnectionStatus()
    {
        return default;
    }

    /// <summary>
    /// <para>Called when the maximum allowed packet size (in bytes) is requested by the <see cref="Godot.MultiplayerApi"/>.</para>
    /// </summary>
    public override int _GetMaxPacketSize()
    {
        return default;
    }

    /// <summary>
    /// <para>Called to get the channel over which the next available packet was received. See <see cref="Godot.MultiplayerPeer.GetPacketChannel()"/>.</para>
    /// </summary>
    public override int _GetPacketChannel()
    {
        return default;
    }

    /// <summary>
    /// <para>Called to get the transfer mode the remote peer used to send the next available packet. See <see cref="Godot.MultiplayerPeer.GetPacketMode()"/>.</para>
    /// </summary>
    public override MultiplayerPeer.TransferModeEnum _GetPacketMode()
    {
        return default;
    }

    /// <summary>
    /// <para>Called when the ID of the <see cref="Godot.MultiplayerPeer"/> who sent the most recent packet is requested (see <see cref="Godot.MultiplayerPeer.GetPacketPeer()"/>).</para>
    /// </summary>
    public override int _GetPacketPeer()
    {
        return default;
    }

    /// <summary>
    /// <para>Called when a packet needs to be received by the <see cref="Godot.MultiplayerApi"/>, if <c>_get_packet</c> isn't implemented. Use this when extending this class via GDScript.</para>
    /// </summary>
    public override byte[] _GetPacketScript()
    {
        return default;
    }

    /// <summary>
    /// <para>Called when the transfer channel to use is read on this <see cref="Godot.MultiplayerPeer"/> (see <see cref="Godot.MultiplayerPeer.TransferChannel"/>).</para>
    /// </summary>
    public override int _GetTransferChannel()
    {
        return default;
    }

    /// <summary>
    /// <para>Called when the transfer mode to use is read on this <see cref="Godot.MultiplayerPeer"/> (see <see cref="Godot.MultiplayerPeer.TransferMode"/>).</para>
    /// </summary>
    public override MultiplayerPeer.TransferModeEnum _GetTransferMode()
    {
        return default;
    }

    /// <summary>
    /// <para>Called when the unique ID of this <see cref="Godot.MultiplayerPeer"/> is requested (see <see cref="Godot.MultiplayerPeer.GetUniqueId()"/>). The value must be between <c>1</c> and <c>2147483647</c>.</para>
    /// </summary>
    public override int _GetUniqueId()
    {
        return default;
    }

    /// <summary>
    /// <para>Called when the "refuse new connections" status is requested on this <see cref="Godot.MultiplayerPeer"/> (see <see cref="Godot.MultiplayerPeer.RefuseNewConnections"/>).</para>
    /// </summary>
    public override bool _IsRefusingNewConnections()
    {
        return default;
    }

    /// <summary>
    /// <para>Called when the "is server" status is requested on the <see cref="Godot.MultiplayerApi"/>. See <see cref="Godot.MultiplayerApi.IsServer()"/>.</para>
    /// </summary>
    public override bool _IsServer()
    {
        return default;
    }

    /// <summary>
    /// <para>Called to check if the server can act as a relay in the current configuration. See <see cref="Godot.MultiplayerPeer.IsServerRelaySupported()"/>.</para>
    /// </summary>
    public override bool _IsServerRelaySupported()
    {
        return default;
    }

    /// <summary>
    /// <para>Called when the <see cref="Godot.MultiplayerApi"/> is polled. See <see cref="Godot.MultiplayerApi.Poll()"/>.</para>
    /// </summary>
    public override void _Poll()
    {
    }

    /// <summary>
    /// <para>Called when a packet needs to be sent by the <see cref="Godot.MultiplayerApi"/>, if <c>_put_packet</c> isn't implemented. Use this when extending this class via GDScript.</para>
    /// </summary>
    public override Error _PutPacketScript(byte[] pBuffer)
    {
        return default;
    }

    /// <summary>
    /// <para>Called when the "refuse new connections" status is set on this <see cref="Godot.MultiplayerPeer"/> (see <see cref="Godot.MultiplayerPeer.RefuseNewConnections"/>).</para>
    /// </summary>
    public override void _SetRefuseNewConnections(bool pEnable)
    {
    }

    /// <summary>
    /// <para>Called when the target peer to use is set for this <see cref="Godot.MultiplayerPeer"/> (see <see cref="Godot.MultiplayerPeer.SetTargetPeer(int)"/>).</para>
    /// </summary>
    public override void _SetTargetPeer(int pPeer)
    {
    }

    /// <summary>
    /// <para>Called when the channel to use is set for this <see cref="Godot.MultiplayerPeer"/> (see <see cref="Godot.MultiplayerPeer.TransferChannel"/>).</para>
    /// </summary>
    public override void _SetTransferChannel(int pChannel)
    {
    }

    /// <summary>
    /// <para>Called when the transfer mode is set on this <see cref="Godot.MultiplayerPeer"/> (see <see cref="Godot.MultiplayerPeer.TransferMode"/>).</para>
    /// </summary>
    public override void _SetTransferMode(MultiplayerPeer.TransferModeEnum pMode)
    {
    }
}