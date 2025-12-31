using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using ReFMGame.Animations;
using ReFMGame.Animations.Camera;
using ReFMGame.Animations.Jumpscare;
using ReFMGame.GameHelper;
using ReFMGame.Network;
using System;
using System.Diagnostics;
using System.Linq;
using Timer = System.Timers.Timer;

namespace ReFMGame.Scenes;

public class Office(FMGame game) : GameScreen(game)
{
    private Texture2D bg_texture;
    private Texture2D camera_texture;
    private Texture2D fan_texture;
    private Texture2D usage_texture;
    private Texture2D left_door_texture;
    private Texture2D right_door_texture;
    private Texture2D cam_flip;
    private Texture2D cam_frame;
    private Texture2D cam_rec_texture;
    private FrameAnimation cam_rec;
    private Texture2D cam_map_texture;
    private Texture2D cam_button;
    private Texture2D cam_disabled;
    private FrameAnimation cam_map;
    private TextureAnimation cam_blip_anim;
    private TextureAnimation cam_up_anim;
    private TextureAnimation cam_down_anim;
    private SoundEffect cam_blip_sound;
    private SoundEffect cam_up_sound;
    private SoundEffect cam_down_sound;
    private FrameAnimation fan_anim;
    private FrameAnimation usage_meter;
    private TextureAnimation static_animation;
    private Effect perspective;
    private float officePosition = 212;
    private float cameraPosition = 0;
    RenderTarget2D officeTarget;
    private SoundEffectInstance fan_sound;
    private SoundEffectInstance light_sound;
    private SoundEffect nose;
    private SoundEffect error;
    private SoundEffect door;
    private SoundEffect power_sound;
    private SoundEffect power_ambience;
    private SoundEffect musicbox;
    private SoundEffect deepsteps;
    private SoundEffect windowscare;
    private SoundEffect scream;
    private SoundEffect foxy_run;
    private SoundEffect knock;

    private BitmapFont smallFont;
    private BitmapFont largeFont;

    private TextureAnimation leftPanel;
    private TextureAnimation rightPanel;
    private TextureAnimation officeControl;

    private TextureAnimation leftDoorClosing;
    private TextureAnimation leftDoorOpening;
    private TextureAnimation leftDoorActive;
    private TextureAnimation rightDoorClosing;
    private TextureAnimation rightDoorOpening;
    private TextureAnimation rightDoorActive;

    private readonly Rectangle NosePos = new(675, 234, 8, 8);
    private readonly Rectangle LeftDoorPos = new(25, 252, 62, 120);
    private readonly Rectangle LeftLightPos = new(26, 393, 62, 120);
    private readonly Rectangle RightDoorPos = new(1519, 268, 62, 120);
    private readonly Rectangle RightLightPos = new(1520, 398, 62, 120);
    private readonly Rectangle CamFlipStart = new(170, 650, 792, 82);
    private readonly Rectangle CamFlipReset = new(98, 550, 1070, 82);

    private bool GameReady = false;

    private readonly (Character, TextureAnimation)[] JumpscareList = new (Character, TextureAnimation)[4];
    private TextureAnimation ActiveJumpscare;
    private Character Jumpscared = Character.None;
    private bool IsJumpscared => Jumpscared != Character.None;
    private bool JumpscareRunning => ActiveJumpscare != null;
    private Character Character = Character.Guard;
    private short Freddy = 0;
    private short Bonnie = 8;
    private short Chica = 1;
    private short Foxy = 2;
    private short Guard = 0;
    private short TargetView = 255;
    private short ActiveView = 255;
    private long CameraData = 0;
    private bool CameraDirection = true;
    private bool BlockCamFlip = false;
    private byte EastHall_light = 0;

    private readonly TextureAnimation[] CameraList = new TextureAnimation[11];
    private Texture2D cam_dark;
    private TextureAnimation cam_foxy_run;

    private readonly SoundEffect[] garbleSounds = new SoundEffect[4];
    private readonly SoundEffect[] robotVocals = new SoundEffect[4];

    private readonly FrameAnimation button_animation = new CamButton();
    // -29;-19
    private readonly Rectangle[] CameraButtons = [
        new(952,325, 60,40),    //1a
        new(934,375, 60,40),    //1b
        new(905,456, 60,40),    //1c
        new(957,575, 60,40),    //2a
        new(957,615, 60,40),    //2b
        new(871,548, 60,40),    //3
        new(1060,575, 60,40),   //4a
        new(1060,615, 60,40),   //4b
        new(832,404, 60,40),    //5
        new(1159,537, 60,40),   //6
        new(1167,407, 60,40)    //7
    ];

    // +7;+7
    private TextureAnimation button_text;
    private TextureAnimation location_text;

    private KeyboardListener _keyboardListener;
    private MouseListener _mouseListener;

    public override void Draw(GameTime gameTime)
	{
		GraphicsDevice.SetRenderTarget(officeTarget);
        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied);
		// Draw targets behind the perspective effect here!
        if (!CameraActive)
        {
            float clampedPos = Math.Clamp(officePosition, 0, 320);
            game.SpriteBatch.Draw(bg_texture, new(0 - clampedPos, 0), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            if (!JumpscareRunning)
            {
                game.SpriteBatch.Draw(left_door_texture, new(68 - clampedPos, -1), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.01f);
                game.SpriteBatch.Draw(right_door_texture, new(1272 - clampedPos, -1), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.01f);
                if (!PowerDown)
                {
                    game.SpriteBatch.Draw(fan_texture, new(780 - clampedPos, 303), fan_anim[fan_anim.Index], Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.01f);
                    game.SpriteBatch.Draw(leftPanel[leftPanelIndex], new(6 - clampedPos, 263), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.01f);
                    game.SpriteBatch.Draw(rightPanel[rightPanelIndex], new(1497 - clampedPos, 273), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.01f);
                }
            }
        }
        else
        {
            float clampedPos = Math.Clamp(cameraPosition, 0, 320);
            game.SpriteBatch.Draw(camera_texture, new(0 - clampedPos, 0), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
        game.SpriteBatch.End();
		GraphicsDevice.SetRenderTarget(game.RenderTarget);

        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied, effect: perspective);
		game.SpriteBatch.Draw(officeTarget, Vector2.Zero, Color.White);
		game.SpriteBatch.End();

		game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied);
        // Draw overlays here!
        if (!PowerDown && !JumpscareRunning)
        {
            game.SpriteBatch.DrawString(smallFont, "Usage:", new(33, 670), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
            game.SpriteBatch.Draw(usage_texture, new(112, 666), usage_meter[Usage - 1], Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
            string power = $"{Power / 10}".PadLeft(3);
            game.SpriteBatch.DrawString(smallFont, $"Power left:{power}%", new(33, 636), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
            string time = $"{Time} AM";
            float width = largeFont.MeasureString(time).Width;
            game.SpriteBatch.DrawString(largeFont, time, new(game.WindowSize.X - width - 26, 24), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
            if(Character == 0)
                game.SpriteBatch.Draw(cam_flip, new(254, 625), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
        }
        if(cam_down_anim.Running)
            game.SpriteBatch.Draw(cam_down_anim[cam_down_anim.Index], Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
        else if (cam_up_anim.Running)
            game.SpriteBatch.Draw(cam_up_anim[cam_up_anim.Index], Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
        if (CameraActive)
        {
            Color staticcolor = Color.White;
            staticcolor.A = (byte)(255 - StaticOpacity);
            game.SpriteBatch.Draw(static_animation[static_animation.Index], Vector2.Zero, null, staticcolor, 0, Vector2.Zero, 1, SpriteEffects.None, .2f);
            game.SpriteBatch.Draw(cam_frame, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .3f);
            game.SpriteBatch.Draw(cam_rec_texture, new(32,32), cam_rec[cam_rec.Index], Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .3f);
            game.SpriteBatch.Draw(cam_map_texture, new(850,300), cam_map[cam_map.Index], Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .3f);
            game.SpriteBatch.Draw(location_text[ActiveView], new(830, 288), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .3f);
            if(ActiveView == 9)
                game.SpriteBatch.Draw(cam_disabled, new(463, 75), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .3f);
            for (byte i = 0; i < CameraButtons.Length; i++)
            {
                Vector2 btn_pos = CameraButtons[i].Location.ToVector2();
                Vector2 text_pos = btn_pos + new Vector2(7,7);
                game.SpriteBatch.Draw(cam_button, btn_pos, ActiveView == i ? button_animation[button_animation.Index] : button_animation[0], Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .31f);
                game.SpriteBatch.Draw(button_text[i], text_pos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .32f);
            }
            if (cam_blip_anim.Running)
            {
                game.SpriteBatch.Draw(cam_blip_anim[cam_blip_anim.Index], new(0, 0), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
            }
        }
        if (game.DebugMode)
        {
            if (!PowerDown && Character == 0)
            {
                game.SpriteBatch.DrawRectangle(CamFlipReset, new(123, 43, 127), layerDepth: 1);
                game.SpriteBatch.DrawRectangle(CamFlipStart, new(123, 43, 127), layerDepth: 1);
            }
            if (!CameraActive)
            {
                // freddy nose
                game.SpriteBatch.DrawRectangle(Clamp(NosePos), Color.Red, layerDepth: .9f);

                if (!PowerDown)
                {
                    Color leftcontrol = BlockLeft ? Color.Red : Color.Gold;
                    Color rightcontrol = BlockRight ? Color.Red : Color.Gold;

                    // left door
                    game.SpriteBatch.DrawRectangle(Clamp(LeftDoorPos), LeftDoor ? Color.Green : leftcontrol, layerDepth: 1);
                    // left light
                    game.SpriteBatch.DrawRectangle(Clamp(LeftLightPos), LeftLight ? Color.AliceBlue : leftcontrol, layerDepth: 1);

                    // right door
                    game.SpriteBatch.DrawRectangle(Clamp(RightDoorPos), RightDoor ? Color.Green : rightcontrol, layerDepth: 1);
                    // right light
                    game.SpriteBatch.DrawRectangle(Clamp(RightLightPos), RightLight ? Color.AliceBlue : rightcontrol, layerDepth: 1);
                }

                // <- left
                game.SpriteBatch.DrawLine(new(556, 0), new(556, 720), new(123, 43, 127), layerDepth: 1);
                game.SpriteBatch.DrawLine(new(316, 0), new(316, 720), new(99, 23, 99), layerDepth: 1);
                game.SpriteBatch.DrawLine(new(164, 0), new(164, 720), new(71, 7, 75), layerDepth: 1);

                // right ->
                game.SpriteBatch.DrawLine(new(748, 0), new(748, 720), new(123, 43, 127), layerDepth: 1);
                game.SpriteBatch.DrawLine(new(996, 0), new(996, 720), new(99, 23, 99), layerDepth: 1);
                game.SpriteBatch.DrawLine(new(1197, 0), new(1197, 720), new(71, 7, 75), layerDepth: 1);
            }
        }
        game.SpriteBatch.End();
        if (!GameReady)
        {
            GameReady = true;
            PowerTimer.Start();
            GameTimer.Start();
        }
	}

    float OfficeSpeed = 0;

    public override void Update(GameTime gameTime)
	{
        StaticOpacity = 150 + rng.Next(50) + (StaticMultiply * 15);
        if (doorCooldown > 0)
            doorCooldown--;
        if (lightCooldown > 0)
            lightCooldown--;
        _keyboardListener.Update(gameTime);
        _mouseListener.Update(gameTime);
		static_animation.Animate(gameTime);
		fan_anim.Animate(gameTime);
        cam_rec.Animate(gameTime);
        cam_blip_anim.Animate(gameTime);
        cam_up_anim.Animate(gameTime);
        cam_down_anim.Animate(gameTime);
        cam_foxy_run.Animate(gameTime);
        button_animation.Animate(gameTime);
        if (leftDoorActive.Running)
        {
            leftDoorActive.Animate(gameTime);
            left_door_texture = leftDoorActive[leftDoorActive.Index];
        }
        if (rightDoorActive.Running)
        {
            rightDoorActive.Animate(gameTime);
            right_door_texture = rightDoorActive[rightDoorActive.Index];
        }
        if (!CameraActive && !JumpscareRunning)
        {
            float office_check = (float)(officePosition + (OfficeSpeed * gameTime.ElapsedGameTime.TotalSeconds));
            if (office_check > 320)
                office_check = 320;
            else if (office_check < 0)
                office_check = 0;
            else
                UpdatePanelCheck();
            officePosition = office_check;
        }
        if(!CameraTimer.Enabled)
        {
            float camera_check = (float)(cameraPosition + ((CameraDirection ? 60 : -60) * gameTime.ElapsedGameTime.TotalSeconds));
            if (camera_check >= 320)
            {
                CameraTimer.Enabled = true;
                camera_check = 320;
            }
            if (camera_check <= 0)
            {
                CameraTimer.Enabled = true;
                camera_check = 0;
            }
            cameraPosition = camera_check;
        }
        if(LeftLight || RightLight)
            LightFlicker();
        if (PowerDown && MusicBoxState > 0 && MusicBoxState < 3)
            CheckMusicBoxState((float)gameTime.ElapsedGameTime.TotalSeconds);
        CheckFoxy(gameTime);
        if (IsJumpscared)
        {
            if (ActiveJumpscare == null)
            {
                if(Jumpscared == Character.Freddy && !CameraActive)
                {
                    FreddyJumpscareTimeout += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if(CameraActive)
                {
                    ForceJumpscareElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (cam_down_anim.Running || FreddyJumpscareTimeout >= 5f || ForceJumpscareElapsed >= 15f || Jumpscared == Character.Foxy)
                {
                    officePosition = Jumpscared == Character.Foxy ? 0 : 160;
                    ToggleCamera(false);
                    ActiveJumpscare = JumpscareList.First(t => t.Item1 == Jumpscared).Item2;
                    ActiveJumpscare.Reset();
                    robot_vocal?.Stop();
                    robot_vocal = null;
                    game.Audio.Play(scream);
                }
            }
            else
            {
                JumpscareElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                ActiveJumpscare.Animate(gameTime);
                bg_texture = ActiveJumpscare[ActiveJumpscare.Index];
                if (JumpscareElapsed >= 1f)
                {
                    ScreenManager.ReplaceScreen(new StaticScene(game));
                }
            }
        }
        if(cam_garble_sound != null)
        {
            GarbleElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (GarbleElapsed >= 3f)
            {
                GarbleElapsed = 0f;
                cam_garble_sound.Stop();
                cam_garble_sound = null;
                ChangeCameraView(Character == Character.Foxy ? Foxy : ActiveView, false);
            }
        }
    }

    private float ForceJumpscareElapsed = 0f;
    private float FreddyJumpscareTimeout = 0f;
    private float GarbleElapsed = 0f;
    private float JumpscareElapsed = 0f;
    private float MusicBoxElapsed = 0f;
    private float FoxyWaitElapsed = 0f;
    private short FoxyAttempt = 0;

    private void CheckFoxy(GameTime gameTime)
    {
        if(Foxy == 3 && FoxyWaitElapsed < 10f)
        {
            FoxyWaitElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        if(Foxy == 3 && FoxyWaitElapsed >= 10f)
        {
            Foxy++;
        }
        if (Foxy == 4 && FoxyWaitElapsed < 1.67f)
        {
            if(ActiveView == 3 && CameraActive && cam_foxy_run.Running)
                camera_texture = cam_foxy_run[cam_foxy_run.Index];
            FoxyWaitElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        if(Foxy == 4 && FoxyWaitElapsed >= 1.67f)
        {
            FoxyWaitElapsed = 0;
            if (LeftDoor)
            {
                Power -= 10 + FoxyAttempt * 13;
                if (Power < 0)
                    Power = 0;
                CheckPower();
                FoxyAttempt++;
                game.Audio.Play(knock);
                Foxy = (short)rng.Next(2);
                if(Character == Character.Foxy)
                    ChangeCameraView(Foxy);
                if((ActiveView == 2 || ActiveView == 3) && CameraActive)
                {
                    PlayCameraGarble();
                }
                return;
            }
            Foxy = 5;
            Jumpscared = Character.Foxy;
            BlockControls |= 1;
            if (LeftLight)
                ToggleLight(true);
        }
    }

    private void CheckMusicBoxState(float elapsed)
    {
        if(MusicBoxState == 1)
        {
            if (game.Audio.IsPlaying(musicbox.Name))
            {
                MusicBoxElapsed += elapsed;
                if (MusicBoxElapsed >= 0.05f)
                {
                    MusicBoxElapsed = 0f;
                    bg_texture = officeControl[rng.Next(4) + 1 == 1 ? 5 : 6];
                }
                return;
            }
            game.Audio.Play(musicbox, unique: true);
            MusicBoxTimer.Enabled = true;
        }
        else
        {
            MusicBoxElapsed += elapsed;
            if (MusicBoxElapsed >= 0.33f)
            {
                MusicBoxElapsed = 0f;
                MusicBoxState++;
                bg_texture = officeControl[7];
                light_sound.Volume = 0;
                game.Audio.Play(deepsteps);
                MusicBoxTimer.Interval = 2000;
                MusicBoxTimer.Enabled = true;
                return;
            }
            if (game.Audio.IsPlaying(musicbox.Name))
            {
                game.Audio.StopAll(name=>name!="office/doors/light");
            }
            if (rng.Next(2) + 1 == 1)
            {
                bg_texture = officeControl[5];
                light_sound.Volume = .25f;
            }
            else
            {
                bg_texture = officeControl[7];
                light_sound.Volume = 0;
            }
        }
    }

    private void LightFlicker()
    {
        if (rng.Next(10) + 1 == 1)
        {
            bg_texture = officeControl[0];
            light_sound.Volume = 0;
        }
        else
        {
            bg_texture = officeControl[lightIndex];
            light_sound.Volume = 1;
        }
    }

    private int lightIndex = 0;
    private int leftPanelIndex = 0;
    private int rightPanelIndex = 0;
    private int doorCooldown = 0;
    private int lightCooldown = 0;
    private int Usage = 1;

    private void PlayBlip()
    {
        if (!CameraActive) return;
        game.Audio.Play(cam_blip_sound);
        cam_blip_anim.Reset();
    }

    private void ToggleLight(bool left)
    {
        if (!RightLight && !LeftLight)
        {
            Usage++;
        }
        if (left)
        {
            leftPanelIndex += leftPanelIndex % 2 == 0 ? 1 : -1;
            rightPanelIndex += !RightLight ? 0 : -1;
            RightLight = false;
            LeftLight = !LeftLight;
            lightIndex = !LeftLight ? 0 : 1;
            if (Bonnie == 21 && LeftLight)
            {
                if(LeftScare == 0)
                {
                    LeftScare = 10;
                    game.Audio.Play(windowscare);
                }
                lightIndex++;
            }
            light_sound.Volume = !LeftLight ? 0 : 1;
        }
        else
        {
            leftPanelIndex += !LeftLight ? 0 : -1;
            rightPanelIndex += rightPanelIndex % 2 == 0 ? 1 : -1;
            RightLight = !RightLight;
            LeftLight = false;
            lightIndex = !RightLight ? 0 : 3;
            if (Chica == 21 && RightLight)
            {
                if (RightScare == 0)
                {
                    RightScare = 10;
                    game.Audio.Play(windowscare);
                }
                lightIndex++;
            }
            light_sound.Volume = !RightLight ? 0 : 1;
        }
        if (!PowerDown)
            bg_texture = officeControl[lightIndex];
        if (!RightLight && !LeftLight)
        {
            Usage--;
        }
    }

    private void ToggleDoor(bool left, bool right)
    {
        if(left != LeftDoor)
        {
            LeftDoor = left;
            if (LeftDoor)
            {
                leftDoorActive = leftDoorClosing;
                leftDoorClosing.Reset();
                Usage++;
            }
            else
            {
                leftDoorActive = leftDoorOpening;
                leftDoorOpening.Reset();
                Usage--;
            }
            game.Audio.Play(door);
            leftPanelIndex += LeftDoor ? 2 : -2;
        }
        if (right != RightDoor)
        {
            RightDoor = right;
            if (RightDoor)
            {
                rightDoorActive = rightDoorClosing;
                rightDoorClosing.Reset();
                Usage++;
            }
            else
            {
                rightDoorActive = rightDoorOpening;
                rightDoorOpening.Reset();
                Usage--;
            }
            game.Audio.Play(door);
            rightPanelIndex += RightDoor ? 2 : -2;
        }
    }

    public void CheckPower()
    {
        if (Power <= 0 && !PowerDown)
        {
            if (cam_up_anim.Running)
            {
                cam_up_anim.Stop();
                cam_down_anim.Reset();
            }
            ToggleCamera(false);
            PowerTimer.Stop();
            Power = 0;
            bg_texture = officeControl[5];
            PowerDown = true;
            if (LeftLight || RightLight)
                ToggleLight(LeftLight);
            if (LeftDoor)
                leftDoorOpening.AnimationFinished += delegate
                {
                    left_door_texture = new Texture2D(GraphicsDevice, 1, 1);
                    left_door_texture.SetData([Color.Transparent]);
                };
            if(RightDoor)
                rightDoorOpening.AnimationFinished += delegate
                {
                    right_door_texture = new Texture2D(GraphicsDevice, 1, 1);
                    right_door_texture.SetData([Color.Transparent]);
                };
            ToggleDoor(false, false);
            fan_sound.Stop();
            game.Audio.Play(power_sound);
            game.Audio.Play(power_ambience, volume: 0.5f);
            MusicBoxTimer.Enabled = true;
        }
    }

    public void CheckCounter()
    {
        if (TimeCounter >= 86)
        {
            TimeCounter = 1;
            if (Time == 12)
                Time = 1;
            else if (Time == 5)
            {
                Time++;
                GameTimer.Stop();
                ScreenManager.ReplaceScreen(new NextDay(game, game.GetScreenshot()));
            }
            else
                Time++;
        }
    }

    private void CamUpFinish(object s, EventArgs e)
    {
        if (LeftLight || RightLight)
            ToggleLight(LeftLight);
        Usage++;
        CameraActive = true;
        ChangeCameraView(ActiveView);
        if (IsJumpscared)
            PlayRobotVocal();
    }

    private SoundEffectInstance cam_sound;

    private void ToggleCamera(bool active)
    {
        BlockCamFlip = true;
        if (CameraActive == active)
            return;
        
        if (active)
        {
            if(cam_sound != null && !cam_sound.IsDisposed)
                cam_sound.Stop();
            cam_down_anim.Stop();
            cam_up_anim.Reset();
            cam_sound = game.Audio.Play(cam_up_sound);
            fan_sound.Volume = .1f;
        }
        else
        {
            if (cam_sound != null && !cam_sound.IsDisposed)
                cam_sound.Stop();
            cam_sound = game.Audio.Play(cam_down_sound);
            CameraActive = false;
            cam_up_anim.Stop();
            cam_down_anim.Reset();
            fan_sound.Volume = .5f;
            Usage--;
        }
    }

    private void UpdateCamera()
    {
        CameraData = 0;
        short freddy = Freddy;
        if(freddy >= 0 && freddy <= 10)
            CameraData |= 0b001L << (3 * freddy);

        short bonnie = Bonnie;
        if(bonnie >= 0 && bonnie <= 10)
        {
            CameraData |= 0b010L << (3 * bonnie);
            if(freddy == -1)
                CameraData |= 0b001L;
        }

        short chica = Chica;
        if (chica >= 0 && chica <= 10)
        {
            CameraData |= 0b100L << (3 * chica);
            if (freddy == -1)
                CameraData |= 0b001L;
        }

        if (Foxy <= 3)
        {
            CameraData |= ((long)Foxy) << (3 * 2);
        }
        else
        {
            CameraData |= 3L << (3 * 2);
        }
    }

    private void ChangeCameraView(short target, bool blip = true)
    {
        ActiveView = target;
        switch (Character)
        {
            case Character.Guard:
                Guard = target;
                break;
            case Character.Freddy:
                Freddy = target;
                break;
            case Character.Bonnie:
                Bonnie = target;
                break;
            case Character.Chica:
                Chica = target;
                break;
            case Character.Foxy:
                Foxy = target;
                if (Foxy < 3)
                {
                    ActiveView = 2;
                }
                else if (Foxy <= 4)
                {
                    ActiveView = 3;
                }
                else
                {
                    ActiveView = 21;
                }
                break;
        }
        UpdateCamera();
        if (ActiveView <= 10)
        {
            int index = (int)CameraData.ExtractBits(0 + (3 * ActiveView), 2 + (3 * ActiveView));
            if (cam_garble_sound == null || cam_garble_sound.IsDisposed)
            {
                if (Foxy == 3 && ActiveView == 3)
                {
                    if (!cam_foxy_run.Running)
                    {
                        game.Audio.Play(foxy_run);
                        FoxyWaitElapsed = 0;
                        cam_foxy_run.Reset();
                        Foxy++;
                    }
                    camera_texture = cam_foxy_run[cam_foxy_run.Index];
                }
                else
                {
                    camera_texture = CameraList[ActiveView][index];
                }
            }
            else
                camera_texture = cam_dark;
        }
        else
        {
            camera_texture = officeControl[0];
        }
        if (blip)
        {
            PlayBlip();
            button_animation.Reset(1);
        }
    }
    private SoundEffectInstance cam_garble_sound;
    private SoundEffectInstance robot_vocal;
    private void PlayCameraGarble()
    {
        if (!CameraActive || cam_garble_sound != null && !cam_garble_sound.IsDisposed)
            return;
        cam_garble_sound = game.Audio.Play(garbleSounds[rng.Next(garbleSounds.Length)]);
        camera_texture = cam_dark;
    }

    private void PlayRobotVocal()
    {
        robot_vocal = game.Audio.Play(robotVocals[rng.Next(robotVocals.Length)]);
    }

    private bool RightDoor = false;
    private bool RightLight = false;
    private bool LeftDoor = false;
    private bool LeftLight = false;

    private int BlockControls = 0;
    private bool BlockLeft => (BlockControls & 1) == 1;
    private bool BlockRight => (BlockControls & 2) == 2;

    private bool CameraActive = false;

    private int LeftScare = 0;
    private int RightScare = 0;

    private int TimeCounter = 0;
    private int Time = 12;
    private int Power = 999;
    private bool PowerDown = false;

    private byte MusicBoxTry = 0;
    private byte MusicBoxState = 0;

    private int StaticOpacity = 0;
	private int StaticMultiply = 0;
    private Timer ScareTimer;
	private Timer StaticOpacityTimer;
	private Timer PowerTimer;
	private Timer GameTimer;
	private Timer CameraTimer;
	private Timer MusicBoxTimer;
    private readonly Random rng = new(Guid.NewGuid().GetHashCode());

    public void PreLoad(Action<bool> callback)
    {
        _keyboardListener = new KeyboardListener();
        var settings = new MouseListenerSettings();
        settings.DoubleClickMilliseconds = int.MinValue;
        settings.DragThreshold = int.MaxValue;
        _mouseListener = new MouseListener(settings);
        perspective = Content.Load<Effect>("office/shader");
        fan_texture = Content.Load<Texture2D>("office/fan_loop");
        nose = Content.Load<SoundEffect>("FreddyNose");
        error = Content.Load<SoundEffect>("error");
        door = Content.Load<SoundEffect>("office/doors/door");
        power_sound = Content.Load<SoundEffect>("office/powerdown");
        power_ambience = Content.Load<SoundEffect>("office/ambience2");
        musicbox = Content.Load<SoundEffect>("office/musicbox");
        deepsteps = Content.Load<SoundEffect>("office/deepsteps");
        windowscare = Content.Load<SoundEffect>("office/windowscare");
        foxy_run = Content.Load<SoundEffect>("office/run");
        knock = Content.Load<SoundEffect>("office/knock");
        scream = Content.Load<SoundEffect>("jumpscare/xscream");

        cam_frame = Content.Load<Texture2D>("camera/frame");
        cam_rec_texture = Content.Load<Texture2D>("camera/rec");
        cam_map_texture = Content.Load<Texture2D>("camera/map");
        cam_flip = Content.Load<Texture2D>("camera/anim/flip");
        cam_button = Content.Load<Texture2D>("camera/button");
        cam_disabled = Content.Load<Texture2D>("camera/disabled");

        cam_up_anim = new CamUp(Content);
        cam_down_anim = new CamDown(Content);
        cam_blip_anim = new CamBlip(Content);
        button_text = new CamButtonText(Content);
        location_text = new CamLocationText(Content);

        cam_up_anim.AnimationFinished += CamUpFinish;

        cam_blip_sound = Content.Load<SoundEffect>("camera/anim/blip");
        cam_down_sound = Content.Load<SoundEffect>("camera/anim/down");
        cam_up_sound = Content.Load<SoundEffect>("camera/anim/up");

        usage_texture = Content.Load<Texture2D>("office/usage");
        usage_meter = new UsageMeter();

        smallFont = Content.Load<BitmapFont>("font/b_volter20");
        largeFont = Content.Load<BitmapFont>("font/b_volter32");

        CameraList[0] = new ShowStage(Content);
        CameraList[1] = new DiningArea(Content);
        CameraList[2] = new PirateCove(Content);
        CameraList[3] = new WestHall(Content);
        CameraList[4] = new WestHallCorner(Content);
        CameraList[5] = new SupplyCloset(Content);
        CameraList[6] = new EastHall(Content);
        CameraList[7] = new EastHallCorner(Content);
        CameraList[8] = new BackStage(Content);
        CameraList[9] = new Kitchen(Content);
        CameraList[10] = new Restrooms(Content);

        cam_foxy_run = new FoxyRunning(Content);
        cam_dark = Content.Load<Texture2D>("camera/view/black");

        JumpscareList[0] = (Character.Freddy, new JumpFreddy(Content));
        JumpscareList[1] = (Character.Bonnie, new JumpBonnie(Content));
        JumpscareList[2] = (Character.Chica, new JumpChica(Content));
        JumpscareList[3] = (Character.Foxy, new JumpFoxy(Content));

        garbleSounds[0] = Content.Load<SoundEffect>("camera/garble1");
        garbleSounds[1] = Content.Load<SoundEffect>("camera/garble2");
        garbleSounds[2] = Content.Load<SoundEffect>("camera/garble3");
        garbleSounds[3] = Content.Load<SoundEffect>("camera/garble4");

        robotVocals[0] = Content.Load<SoundEffect>("office/vocal1");
        robotVocals[1] = Content.Load<SoundEffect>("office/vocal2");
        robotVocals[2] = Content.Load<SoundEffect>("office/vocal3");
        robotVocals[3] = Content.Load<SoundEffect>("office/vocal4");

        leftPanel = new LeftPanelControl(Content);
        rightPanel = new RightPanelControl(Content);
        officeControl = new OfficeControl(Content);

        rightDoorClosing = new RightDoorClosing(Content);
        rightDoorOpening = new RightDoorOpening(Content);

        leftDoorClosing = new LeftDoorClosing(Content);
        leftDoorOpening = new LeftDoorOpening(Content);

        leftDoorActive = leftDoorClosing;
        left_door_texture = leftDoorActive[0];
        rightDoorActive = rightDoorClosing;
        right_door_texture = rightDoorActive[0];

        bg_texture = officeControl[0];
        camera_texture = CameraList[0][0];
        callback(true);
    }

    public override void LoadContent()
	{
        base.LoadContent();
        officeTarget = new(GraphicsDevice, game.WindowSize.X, game.WindowSize.Y);

        cam_map = new CamMap();
        cam_rec = new CamRec();
        fan_anim = new FanLoop();
        fan_sound = game.Audio.Play(Content.Load<SoundEffect>("office/fan_sound"), volume: 0.5f, isLooped: true);
        light_sound = game.Audio.Play(Content.Load<SoundEffect>("office/doors/light"), volume: 0f, isLooped: true, unique: true);

        if(Character > 0)
        {
            fan_sound.Volume = 0.02f;
            CameraActive = true;
        }
        switch (Character)
        {
            case Character.Guard:
                ChangeCameraView(Guard);
                break;
            case Character.Freddy:
                ChangeCameraView(Freddy);
                break;
            case Character.Bonnie:
                ChangeCameraView(Bonnie);
                break;
            case Character.Chica:
                ChangeCameraView(Chica);
                break;
            case Character.Foxy:
                ChangeCameraView(Foxy);
                break;
        }

        static_animation = new StaticAnim(Content);
		ScareTimer = new Timer(1000);
		ScareTimer.Elapsed += delegate
		{
            if (RightScare > 0) RightScare--;
            if (LeftScare > 0) LeftScare--;
		};
		ScareTimer.AutoReset = true;
		ScareTimer.Enabled = true;

		PowerTimer = new Timer(1000);
		PowerTimer.Elapsed += delegate
		{
            Power -= Usage;
            CheckPower();
        };
		PowerTimer.AutoReset = true;
		PowerTimer.Enabled = false;

        MusicBoxTimer = new Timer(5000);
        MusicBoxTimer.Elapsed += delegate
        {
            MusicBoxTry++;
            Debug.WriteLine($"Music Box State: {MusicBoxState}, Try: {MusicBoxTry}");
            if (MusicBoxState < 3) {
                if (rng.Next(5) + 1 == 1 || MusicBoxTry == 4)
                {
                    MusicBoxTry = 0;
                    MusicBoxState++;
                    MusicBoxTimer.Enabled = false;
                }
            }
            else
            {
                if (rng.Next(5) + 1 == 1 || MusicBoxTry == 10)
                {
                    MusicBoxTry = 0;
                    MusicBoxState++;
                    MusicBoxTimer.Enabled = false;
                    ScreenManager.ReplaceScreen(new FreddyScene(game));
                }
            }
        };
        MusicBoxTimer.AutoReset = true;
        MusicBoxTimer.Enabled = false;

        GameTimer = new Timer(1000);
        GameTimer.Elapsed += delegate
        {
            TimeCounter++;
            CheckCounter();
            EastHall_light = (byte)rng.Next(2);
            if(ActiveView == 3 && (cam_garble_sound == null || cam_garble_sound.IsDisposed))
            {
                int index = (int)CameraData.ExtractBits(0 + (3 * ActiveView), 2 + (3 * ActiveView));
                camera_texture = CameraList[ActiveView][index+EastHall_light];
            }
        };
        GameTimer.AutoReset = true;
        GameTimer.Enabled = false;

        CameraTimer = new Timer(4000);
        CameraTimer.Elapsed += delegate
        {
            CameraDirection = !CameraDirection;
            CameraTimer.Enabled = false;
        };
        CameraTimer.AutoReset = true;
        CameraTimer.Enabled = false;

        StaticMultiply = rng.Next(3);
        StaticOpacity = 150 + rng.Next(50) + (StaticMultiply * 15);
		StaticOpacityTimer = new Timer(1000);
		StaticOpacityTimer.Elapsed += delegate
		{
            StaticMultiply = rng.Next(3);
        };
        StaticOpacityTimer.AutoReset = true;
		StaticOpacityTimer.Enabled = true;

        _keyboardListener.KeyPressed += KeyDebug;
        _mouseListener.MouseClicked += MouseClick;
        _mouseListener.MouseMoved += MouseMove;
    }

    private void UpdatePanelCheck()
    {
        byte control = 0;
        bool left = Clamp(LeftLightPos).Contains(game.MouseState.Position) || Clamp(LeftDoorPos).Contains(game.MouseState.Position);
        bool door = Clamp(RightDoorPos).Contains(game.MouseState.Position) || Clamp(LeftDoorPos).Contains(game.MouseState.Position);
        if (left || Clamp(RightLightPos).Contains(game.MouseState.Position) || Clamp(RightDoorPos).Contains(game.MouseState.Position))
        {
            control |= 1;
            if (left)
                control |= (1 << 1);
            if (door)
                control |= (1 << 2);
        }
        ControlPanelCheck = control;
    }

    // [door?][left?][active?]
    // leftlight 3
    // rightlight 1
    // leftdoor 7
    // rightdoor 5
    private byte ControlPanelCheck = 0;

    private void MouseMove(object sender, MouseEventArgs e)
    {
        ControlPanelCheck = 0;
        OfficeSpeed = 0;

        if(JumpscareRunning)
            return;

        if (CameraActive)
        {
            if (!BlockCamFlip && CamFlipStart.Contains(game.MouseState.Position) && Character == 0)
            {
                ToggleCamera(false);
            }
            else if (CamFlipReset.Contains(game.MouseState.Position))
                BlockCamFlip = false;
            byte target = 255;
            for(byte i = 0; i < CameraButtons.Length; i++)
            {
                if (CameraButtons[i].Contains(game.MouseState.Position))
                {
                    target = i;
                    break;
                }
            }
            TargetView = target;
            return;
        }
        else if (!BlockCamFlip && !PowerDown && CamFlipStart.Contains(game.MouseState.Position))
        {
            ToggleCamera(true);
            return;
        }
        else if (CamFlipReset.Contains(game.MouseState.Position) && !cam_up_anim.Running)
            BlockCamFlip = false;

        UpdatePanelCheck();

        float x = game.MouseState.Position.X;
        if (x > 748)
        {
            OfficeSpeed += 120;
            if (x > 996)
            {
                OfficeSpeed += 300;
                if (x > 1197)
                {
                    OfficeSpeed += 300;
                }
            }
        }
        else if (x < 556)
        {
            OfficeSpeed -= 120;
            if (x < 316)
            {
                OfficeSpeed -= 300;

                if (x < 164)
                {
                    OfficeSpeed -= 300;
                }
            }
        }
    }

    private void MouseClick(object sender, MouseEventArgs e)
    {
        if(JumpscareRunning)
            return;
        if (e.Button == MouseButton.Left)
        {
            if (CameraActive)
            {
                if(TargetView != 255 && CameraButtons[TargetView].Contains(game.MouseState.Position))
                {
                    ChangeCameraView(TargetView);
                }
                return;
            }
            if (Clamp(NosePos).Contains(game.MouseState.Position))
            {
                nose.Play(1f, 0, 0);
            }
            if (PowerDown) return;
            switch (ControlPanelCheck)
            {
                case 1:
                    if (lightCooldown > 0) return;
                    lightCooldown = 2;
                    if (BlockRight)
                    {
                        error.Play(1f, 0, 0);
                        return;
                    }
                    ToggleLight(false);
                    return;
                case 3:
                    if (lightCooldown > 0) return;
                    lightCooldown = 2;
                    if (BlockLeft)
                    {
                        error.Play(1f, 0, 0);
                        return;
                    }
                    ToggleLight(true);
                    return;
                case 5:
                    if (doorCooldown > 0) return;
                    doorCooldown = 10;
                    if (BlockRight)
                    {
                        error.Play(1f, 0, 0);
                        return;
                    }
                    ToggleDoor(LeftDoor, !RightDoor);
                    return;
                case 7:
                    if (doorCooldown > 0) return;
                    doorCooldown = 10;
                    if (BlockLeft)
                    {
                        error.Play(1f, 0, 0);
                        return;
                    }
                    ToggleDoor(!LeftDoor, RightDoor);
                    return;
            }
        }
    }

    private void KeyDebug(object s, KeyboardEventArgs e)
    {
        if (!game.DebugMode || JumpscareRunning)
            return;
        if (!CameraActive && KeyboardExtended.GetState().IsKeyDown(Keys.B))
        {
            switch (e.Key)
            {
                case Keys.D0:
                    BlockControls = BlockLeft || BlockRight ? BlockControls & ~3 : BlockControls | 3;
                    if (LeftLight || RightLight)
                        ToggleLight(LeftLight);
                    ToggleDoor(false, false);
                    break;
                case Keys.D1:
                    BlockControls = BlockLeft ? BlockControls & ~1 : BlockControls | 1;
                    if (LeftLight)
                        ToggleLight(true);
                    ToggleDoor(false, RightDoor);
                    break;
                case Keys.D2:
                    BlockControls = BlockRight ? BlockControls & ~2 : BlockControls | 2;
                    if (RightLight)
                        ToggleLight(false);
                    ToggleDoor(LeftDoor, false);
                    break;
            }
            return;
        }
        switch (e.Key)
        {
            case Keys.P:
                Power = 30;
                break;
            case Keys.T:
                Time = 5;
                TimeCounter = 80;
                break;
            case Keys.M:
                if (Foxy < 3)
                {
                    if(ActiveView == 2)
                        PlayCameraGarble();
                    Foxy++;
                    if (Foxy == 3 && ActiveView == 3)
                        PlayCameraGarble();
                }
                break;
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
        CameraTimer?.Dispose();
        GameTimer?.Dispose();
        ScareTimer?.Dispose();
        PowerTimer?.Dispose();
        StaticOpacityTimer?.Dispose();
        MusicBoxTimer?.Dispose();
        _keyboardListener.KeyPressed -= KeyDebug;
        _mouseListener.MouseClicked -= MouseClick;
        _mouseListener.MouseMoved -= MouseMove;
        cam_up_anim.AnimationFinished -= CamUpFinish;
    }

    private Rectangle Clamp(Rectangle r)
    {
        r.X -= (int)officePosition;
        return r;
    }
}
