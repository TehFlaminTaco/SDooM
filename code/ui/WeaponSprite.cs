using Sandbox;
using Sandbox.UI;

public class WeaponSprite : Panel {
    Panel gunPanel;
	Panel flashPanel;
    public WeaponSprite(){
        gunPanel = Add.Panel("gun");
        gunPanel.Style.BackgroundImage = TextureLoader2.Instance.GetUITexture("PISGA0");
        gunPanel.Style.Width = gunPanel.Style.BackgroundImage.Width;
        gunPanel.Style.Height = gunPanel.Style.BackgroundImage.Height;
        gunPanel.Style.Dirty();

		flashPanel = gunPanel.Add.Panel("flash");
    }

	public void SetSprite(string sprite){
		gunPanel.Style.BackgroundImage = TextureLoader2.Instance.GetUITexture(sprite);
		ResizeChildren(DoomHud.LastHudScale);
	}

	public void SetFlash(string sprite, int left, int top){
		if(sprite!=null){
			flashPanel.Style.BackgroundImage = TextureLoader2.Instance.GetUITexture(sprite);
			flashPanel.Style.Width = flashPanel.Style.BackgroundImage.Width * DoomHud.LastHudScale;
			flashPanel.Style.Height = flashPanel.Style.BackgroundImage.Height * DoomHud.LastHudScale;
			flashPanel.Style.Left = left * DoomHud.LastHudScale;
			flashPanel.Style.Top = -top * DoomHud.LastHudScale;
			flashPanel.Style.Dirty();
		}else{
			flashPanel.Style.BackgroundImage = null;
			flashPanel.Style.Dirty();
		}
		
	}

    public void ResizeChildren(float s){
        gunPanel.Style.Width = gunPanel.Style.BackgroundImage.Width * s;
        gunPanel.Style.Height = gunPanel.Style.BackgroundImage.Height * s;
        gunPanel.Style.Dirty();
    }

    [Event.Frame]
    public void BobView(){
        var Rotation = Local.Pawn.EyeRotation;
        var newPitch = Rotation.Pitch();
		var newYaw = Rotation.Yaw();

		var pitchDelta = Angles.NormalizeAngle( newPitch - lastPitch );
		var yawDelta = Angles.NormalizeAngle( lastYaw - newYaw );

		var verticalDelta = Local.Pawn.Velocity.z * Time.Delta;
		var viewDown = Rotation.FromPitch( newPitch ).Up * -1.0f;
		verticalDelta *= 1.0f - System.MathF.Abs( viewDown.Cross( Vector3.Down ).y );
		pitchDelta -= verticalDelta * 1;
        var offset = Vector3.Zero;
        offset += CalcSwingOffset(pitchDelta, yawDelta);
        offset += CalcBobbingOffset(Local.Pawn.Velocity);
		if(Local.Pawn is DoomPlayer ply && ply.ActiveChild is DoomGun gn && ply.CameraMode is FirstPersonCamera){
			offset.y += gn.pixelOffset / 8f;
			offset.z -= gn.weaponDown / 8f;
			if(gn.weaponStowed){
				offset.z -= 512f/8f * gn.stowStart;
			}else if(gn.weaponDrawn && gn.drawStart < 0.25f){
				offset.z -= 512f/8f * (0.25f-gn.drawStart);
			}
		}else{
			offset.z -= 100000f;
		}
        Style.Left = offset.y * 8f * DoomHud.LastHudScale;
        Style.Bottom = System.Math.Max(0f, -offset.z * 8f)  * DoomHud.LastHudScale;
        lastPitch = newPitch;
        lastYaw = newYaw;
    }

    // Chunked out of ViewModel, lets us bob around.
    protected float SwingInfluence => 0.05f;
	protected float ReturnSpeed => 5.0f;
	protected float MaxOffsetLength => 10.0f;
	protected float BobCycleTime => 7;
	protected Vector3 BobDirection => new Vector3( 0.0f, 1.0f, 0.5f );
	public bool EnableSwingAndBob = true;

	private Vector3 swingOffset;
	private float lastPitch;
	private float lastYaw;
	private float bobAnim;

    protected Vector3 CalcSwingOffset( float pitchDelta, float yawDelta )
	{
		Vector3 swingVelocity = new Vector3( 0, yawDelta, pitchDelta );

		swingOffset -= swingOffset * ReturnSpeed * Time.Delta;
		swingOffset += (swingVelocity * SwingInfluence);

		if ( swingOffset.Length > MaxOffsetLength )
		{
			swingOffset = swingOffset.Normal * MaxOffsetLength;
		}

		return swingOffset;
	}

	protected Vector3 CalcBobbingOffset( Vector3 velocity )
	{
		bobAnim += Time.Delta * BobCycleTime;

		var twoPI = System.MathF.PI * 2.0f;

		if ( bobAnim > twoPI )
		{
			bobAnim -= twoPI;
		}

		var speed = new Vector2( velocity.x, velocity.y ).Length;
		speed = speed > 10.0 ? speed : 0.0f;
		var offset = BobDirection * (speed * 0.005f) * System.MathF.Cos( bobAnim );
		offset = offset.WithZ( -System.MathF.Abs( offset.z ) );

		return offset;
	}
}