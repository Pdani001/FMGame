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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace ReFMGame.Scenes;

public class Office(FMGame game, Character character) : GameScreen(game)
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
    private SoundEffect powerdown_sound;
    private SoundEffect powerdown_ambience;
    private SoundEffect game_ambience;
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
    private readonly Rectangle AttackPos = new(1018, 605, 32, 56);

    private readonly (Character, TextureAnimation)[] JumpscareList = new (Character, TextureAnimation)[4];
    private TextureAnimation ActiveJumpscare;
    private Character Jumpscared = Character.None;
    private bool IsJumpscared => Jumpscared != Character.None;
    private bool JumpscareRunning => ActiveJumpscare != null;
    private Character Character { get; } = character == Character.None ? Character.Guard : character;
    private short Freddy = 0;
    private short Bonnie = -1;
    private short Chica = -1;
    private short Foxy = 0;
    private short Guard = 0;
    private short TargetView = 255;
    private short ActiveView = 255;
    private long CameraData = 0;
    private bool CameraDirection = true;
    private bool BlockCamFlip = false;
    private byte EastHall_light = 0;
    private int MoveTime = 99;

    private GameState GameState;

    private readonly TextureAnimation[] CameraList = new TextureAnimation[11];
    private Texture2D cam_dark;
    private TextureAnimation cam_foxy_run;

    private readonly SoundEffect[] garbleSounds = new SoundEffect[4];
    private readonly SoundEffect[] robotVocals = new SoundEffect[4];
    private readonly SoundEffect[] kitchenOven = new SoundEffect[4];
    private SoundEffectInstance ovenSound;
    private readonly SoundEffect[] freddyLaugh = new SoundEffect[3];
    private SoundEffectInstance freddyMusic;

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

    private readonly Dictionary<Character, Vector2[]> RobotCheatPosition = new()
    {
        {
            Character.Freddy,
            [
                new(-5,-10),
                new(-15,-10),
                new(0,0),
                new(0,0),
                new(0,0),
                new(0,0),
                new(0,-20),
                new(15,15),
                new(0,0),
                new(0,-10),
                new(0,-10),
            ]
        },
        {
            Character.Bonnie,
            [
                new(-15,10),
                new(-10,10),
                new(0,0),
                new(0,-10),
                new(-15,10),
                new(0,0),
                new(0,0),
                new(0,0),
                new(0,0),
                new(0,0),
                new(0,0),
            ]
        },
        {
            Character.Chica,
            [
                new(10,10),
                new(15,10),
                new(0,0),
                new(0,0),
                new(0,0),
                new(0,0),
                new(20,-10),
                new(20,10),
                new(0,0),
                new(0,10),
                new(0,10),
            ]
        },
        {
            Character.Foxy,
            [
                new(0,0),
                new(0,0),
                new(-20,-10),
                new(0,-10),
                new(0,0),
                new(0,0),
                new(0,0),
                new(0,0),
                new(0,0),
                new(0,0),
                new(0,0),
            ]
        },
    };

    private readonly Dictionary<Character, Color> RobotColor = new()
    {
        { Character.Freddy, new(183, 103, 67) },
        { Character.Bonnie, new(59, 123, 163) },
        { Character.Chica, new(223, 203, 0) },
        { Character.Foxy, new(231, 0, 0) },
    };

    // +7;+7
    private TextureAnimation button_text;
    private TextureAnimation location_text;

    private KeyboardListener _keyboardListener;
    private MouseListener _mouseListener;

    private readonly FreddyScene FreddyScene = new(game);

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
            if (Character == Character.Guard)
            {
                game.SpriteBatch.DrawString(smallFont, "Usage:", new(33, 670), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
                game.SpriteBatch.Draw(usage_texture, new(112, 666), usage_meter[Usage - 1], Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
            }
            else
            {
                game.SpriteBatch.DrawString(smallFont, $"You are {Character}", new(33, 602), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
                if (ActiveView > 10 || (Character == Character.Foxy && ActiveView != 2))
                {
                    game.SpriteBatch.DrawString(smallFont, $"Attack in progress, stand by!", new(33, 670), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
                }
                else
                {
                    if (MoveTime > 0)
                    {
                        string move = $"{MoveTime}".PadLeft(2);
                        string padding = MoveTime > 1 ? "s" : "";
                        game.SpriteBatch.DrawString(smallFont, $"You can move in {move} second{padding}", new(33, 670), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
                    }
                    else
                    {
                        game.SpriteBatch.DrawString(smallFont, $"You can move now!", new(33, 670), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
                    }
                }
            }
            string power = $"{Power / 10}".PadLeft(3);
            game.SpriteBatch.DrawString(smallFont, $"Power left:{power}%", new(33, 636), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
            string time = $"{Time} AM";
            float width = largeFont.MeasureString(time).Width;
            game.SpriteBatch.DrawString(largeFont, time, new(game.WindowSize.X - width - 26, 24), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.8f);
            if(Character == Character.Guard)
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
            if (!PowerDown)
            {
                game.SpriteBatch.Draw(cam_map_texture, new(850, 300), cam_map[cam_map.Index], Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .3f);
                game.SpriteBatch.Draw(location_text[ActiveView], new(830, 288), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .3f);
                if (ActiveView == 9)
                    game.SpriteBatch.Draw(cam_disabled, new(463, 75), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .3f);
                for (byte i = 0; i < CameraButtons.Length; i++)
                {
                    Vector2 btn_pos = CameraButtons[i].Location.ToVector2();
                    Vector2 text_pos = btn_pos + new Vector2(7, 7);
                    game.SpriteBatch.Draw(cam_button, btn_pos, ActiveView == i ? button_animation[button_animation.Index] : button_animation[0], Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .31f);
                    game.SpriteBatch.Draw(button_text[i], text_pos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .32f);
                }
            }
            if (cam_blip_anim.Running)
            {
                game.SpriteBatch.Draw(cam_blip_anim[cam_blip_anim.Index], new(0, 0), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
            }
        }
        if (game.DebugMode)
        {
            if (!PowerDown && Character == Character.Guard)
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
            else if (Character != Character.Guard && !IsJumpscared)
            {
                game.SpriteBatch.DrawRectangle(AttackPos, new(95, 155, 0), layerDepth: 1);
            }
        }
        if(Character != Character.Guard || game.DebugMode)
        {
            for (int i = 1; i < 5; i++)
            {
                Character robot = (Character)i;
                if (RobotCheatPosition.TryGetValue(robot, out var relative))
                {
                    short robotPos = robot switch
                    {
                        Character.Freddy => Freddy,
                        Character.Bonnie => Bonnie,
                        Character.Chica => Chica,
                        Character.Foxy => Foxy < 3 ? (short)2 : Foxy < 5 ? (short)3 : (short)21,
                        _ => -1
                    };
                    if (robotPos > -1)
                    {
                        Vector2 iconPos = robotPos >= 21 ? AttackPos.Center.ToVector2() - new Vector2(17) : CameraButtons[robotPos].Center.ToVector2() - new Vector2(17) + relative[robotPos];
                        game.SpriteBatch.FillRectangle(new(iconPos, new(35, 35)), RobotColor[robot], .4f);
                    }
                }
            }
        }
        game.SpriteBatch.End();
	}

    float OfficeSpeed = 0;

    public override void Update(GameTime gameTime)
	{
        if (_keyboardListener == null || _mouseListener == null)
            return;
        StaticOpacity = 150 + rng.Next(50) + (StaticMultiply * 15);
        if (doorCooldown > 0)
            doorCooldown--;
        if (lightCooldown > 0)
            lightCooldown--;
        _keyboardListener.Update(gameTime);
        _mouseListener.Update(gameTime);
		static_animation.Animate(gameTime);
		fan_anim.Animate(gameTime);
        if(Character == Character.Guard || GameState?.Camera.Active == true)
            cam_rec.Animate(gameTime);
        if(Character != Character.Guard && GameState?.Camera.Active == false && cam_rec.Index != 1)
            cam_rec.Reset(1);
        cam_blip_anim.Animate(gameTime);
        cam_up_anim.Animate(gameTime);
        cam_down_anim.Animate(gameTime);
        cam_foxy_run.Animate(gameTime);
        button_animation.Animate(gameTime);
        if (Character == Character.Guard)
        {
            if (leftDoorActive?.Running == true)
            {
                leftDoorActive.Animate(gameTime);
                left_door_texture = leftDoorActive[leftDoorActive.Index];
            }
            if (rightDoorActive?.Running == true)
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
            if (LeftLight || RightLight)
                LightFlicker();
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
        if (PowerDown && MusicBoxState > 0 && MusicBoxState < 3)
            UpdateMusicBox((float)gameTime.ElapsedGameTime.TotalSeconds);
        if (ActiveView == 3 && CameraActive && cam_foxy_run.Running)
            camera_texture = cam_foxy_run[cam_foxy_run.Index];
        if (IsJumpscared && ActiveJumpscare != null)
        {
            ActiveJumpscare.Animate(gameTime);
            bg_texture = ActiveJumpscare[ActiveJumpscare.Index];
        }
    }

    private float _musicboxElapsed = 0f;
    private bool MusicBoxRunning = false;

    private void UpdateMusicBox(float elapsed)
    {
        if(MusicBoxState == 1)
        {
            if (MusicBoxRunning)
            {
                _musicboxElapsed += elapsed;
                if (_musicboxElapsed >= 0.05f)
                {
                    _musicboxElapsed -= .05f;
                    bg_texture = officeControl[rng.Next(4) + 1 == 1 ? 5 : 6];
                    camera_texture = bg_texture;
                }
                return;
            }
        }
        else
        {
            if (game.Audio.IsPlaying(musicbox.Name))
            {
                game.Audio.StopAll(name=>name!="office/doors/light");
            }
            if (rng.Next(2) + 1 == 1)
            {
                bg_texture = officeControl[5];
                camera_texture = bg_texture;
                light_sound.Volume = .25f;
            }
            else
            {
                bg_texture = officeControl[7];
                camera_texture = bg_texture;
                light_sound.Volume = 0;
            }
        }
    }

    private void LightFlicker()
    {
        if (rng.Next(10) + 1 == 1)
        {
            bg_texture = officeControl[0];
            if (light_sound != null)
                light_sound.Volume = 0;
        }
        else
        {
            bg_texture = officeControl[lightIndex];
            if (light_sound != null)
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
        if (Character != Character.Guard)
            return;
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
                    game.Audio.Play(windowscare, .6f);
                }
                lightIndex++;
            }
            if (light_sound != null)
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
                    game.Audio.Play(windowscare, .6f);
                }
                lightIndex++;
            }
            if (light_sound != null)
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
        if (Character != Character.Guard)
            return;
        if (left != LeftDoor)
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

    private void CheckPower()
    {
        if (Power <= 0 && !PowerDown)
        {
            if (cam_up_anim.Running)
            {
                cam_up_anim.Stop();
                cam_down_anim.Reset();
            }
            ToggleCamera(false);
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
            fan_sound?.Dispose();
            freddyMusic?.Dispose();
            ovenSound?.Dispose();
            game.Audio.Play(powerdown_sound, Character == Character.Guard ? 1f : .15f);
            game.Audio.Play(powerdown_ambience, volume: 0.5f, isLooped: true);
            if (Character != Character.Guard)
                ChangeCameraView(21);
        }
    }

    private void CamUpFinish(object s, EventArgs e)
    {
        if (LeftLight || RightLight)
            ToggleLight(LeftLight);
        Usage++;
        CameraActive = true;
        game.Client.SetCameraActive(CameraActive);
        ChangeCameraView(ActiveView);
        if (Jumpscared == Character.Bonnie || Jumpscared == Character.Chica)
            PlayRobotVocal();
    }

    private SoundEffectInstance cam_sound;

    private void ToggleCamera(bool active)
    {
        if (Character != Character.Guard)
            return;
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
            if(fan_sound != null)
                fan_sound.Volume = .1f;
        }
        else
        {
            if (cam_sound != null && !cam_sound.IsDisposed)
                cam_sound.Stop();
            cam_sound = game.Audio.Play(cam_down_sound);
            CameraActive = false;
            game.Client.SetCameraActive(CameraActive);
            cam_up_anim.Stop();
            cam_down_anim.Reset();
            if(fan_sound != null)
                fan_sound.Volume = .5f;
            Usage--;
            if (ovenSound != null && !ovenSound.IsDisposed)
                ovenSound.Volume = .1f;
            if (freddyMusic != null && !freddyMusic.IsDisposed)
                freddyMusic.Volume = .1f;
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

        if(ActiveView == 9)
        {
            if (ovenSound != null && !ovenSound.IsDisposed)
                ovenSound.Volume = 1;
            if (freddyMusic != null && !freddyMusic.IsDisposed)
                freddyMusic.Volume = 1;
        }
        else
        {
            if (ovenSound != null && !ovenSound.IsDisposed)
                ovenSound.Volume = .1f;
            if (freddyMusic != null && !freddyMusic.IsDisposed)
                freddyMusic.Volume = .1f;
        }
    }

    private void ChangeCameraView(short target, bool blip = true)
    {
        ActiveView = target;
        switch (Character)
        {
            case Character.Guard:
                if(target > 10)
                {
                    target = 10;
                    ActiveView = target;
                }
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
            if (!CameraGarble)
            {
                if(!cam_foxy_run.Running)
                    camera_texture = CameraList[ActiveView][index];
            }
            else
                camera_texture = cam_dark;
        }
        else
        {
            camera_texture = officeControl[!PowerDown ? 0 : 5];
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
        if (!CameraActive || CameraGarble)
            return;
        CameraGarble = true;
        cam_garble_sound = game.Audio.Play(garbleSounds[rng.Next(garbleSounds.Length)]);
        camera_texture = cam_dark;
    }

    private void PlayRobotVocal()
    {
        robot_vocal = game.Audio.Play(robotVocals[rng.Next(robotVocals.Length)], .3f);
    }

    private bool RightDoor = false;
    private bool RightLight = false;
    private bool LeftDoor = false;
    private bool LeftLight = false;

    private bool BlockLeft { get; set; }
    private bool BlockRight { get; set; }

    private bool CameraActive = false;
    private bool CameraGarble = false;

    private int LeftScare = 0;
    private int RightScare = 0;

    private int Time = 12;
    private int Power = 999;
    private bool PowerDown = false;

    private byte MusicBoxState = 0;

    private int StaticOpacity = 0;
	private int StaticMultiply = 0;
    private Timer ScareTimer;
	private Timer StaticOpacityTimer;
	private Timer HallwayTimer;
	private Timer CameraTimer;
    private readonly Random rng = new(Guid.NewGuid().GetHashCode());

    public void SetPositions(CharacterPosition[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            var position = positions[i].Position;
            switch (positions[i].Character)
            {
                case Character.Freddy:
                    Freddy = position;
                    break;
                case Character.Bonnie:
                    Bonnie = position;
                    break;
                case Character.Chica:
                    Chica = position;
                    break;
                case Character.Foxy:
                    Foxy = position;
                    break;
            }
        }
    }

    public void PreLoad(Action<bool> callback)
    {
        _keyboardListener = new KeyboardListener();
        var settings = new MouseListenerSettings
        {
            DoubleClickMilliseconds = int.MinValue,
            DragThreshold = int.MaxValue
        };
        _mouseListener = new MouseListener(settings);
        perspective = Content.Load<Effect>("office/shader");
        fan_texture = Content.Load<Texture2D>("office/fan_loop");

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

        if (!game.Audio.NoAudio)
        {
            nose = Content.Load<SoundEffect>("FreddyNose");
            error = Content.Load<SoundEffect>("error");
            door = Content.Load<SoundEffect>("office/doors/door");
            powerdown_sound = Content.Load<SoundEffect>("office/powerdown");
            powerdown_ambience = Content.Load<SoundEffect>("office/power_ambience");
            game_ambience = Content.Load<SoundEffect>("office/ambience");
            musicbox = Content.Load<SoundEffect>("office/musicbox");
            deepsteps = Content.Load<SoundEffect>("office/deepsteps");
            windowscare = Content.Load<SoundEffect>("office/windowscare");
            foxy_run = Content.Load<SoundEffect>("office/run");
            knock = Content.Load<SoundEffect>("office/knock");
            scream = Content.Load<SoundEffect>("jumpscare/xscream");

            cam_blip_sound = Content.Load<SoundEffect>("camera/anim/blip");
            cam_down_sound = Content.Load<SoundEffect>("camera/anim/down");
            cam_up_sound = Content.Load<SoundEffect>("camera/anim/up");

            garbleSounds[0] = Content.Load<SoundEffect>("camera/garble1");
            garbleSounds[1] = Content.Load<SoundEffect>("camera/garble2");
            garbleSounds[2] = Content.Load<SoundEffect>("camera/garble3");
            garbleSounds[3] = Content.Load<SoundEffect>("camera/garble4");

            kitchenOven[0] = Content.Load<SoundEffect>("camera/oven1");
            kitchenOven[1] = Content.Load<SoundEffect>("camera/oven2");
            kitchenOven[2] = Content.Load<SoundEffect>("camera/oven3");
            kitchenOven[3] = Content.Load<SoundEffect>("camera/oven4");

            robotVocals[0] = Content.Load<SoundEffect>("office/vocal1");
            robotVocals[1] = Content.Load<SoundEffect>("office/vocal2");
            robotVocals[2] = Content.Load<SoundEffect>("office/vocal3");
            robotVocals[3] = Content.Load<SoundEffect>("office/vocal4");

            freddyLaugh[0] = Content.Load<SoundEffect>("office/laugh1d");
            freddyLaugh[1] = Content.Load<SoundEffect>("office/laugh2d");
            freddyLaugh[2] = Content.Load<SoundEffect>("office/laugh8d");
        }

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

        FreddyScene.PreLoad();
        Task.Delay(10).ContinueWith(t =>
        {
            callback?.Invoke(true);
        });
    }

    public override void LoadContent()
	{
        if (!game.Client.IsConnected)
        {
            Client_Disconnected("");
            return;
        }
        officeTarget = new(GraphicsDevice, game.WindowSize.X, game.WindowSize.Y);

        cam_map = new CamMap();
        cam_rec = new CamRec();
        fan_anim = new FanLoop();
        if (!game.Audio.NoAudio)
        {
            var fanSoundEffect = Content.Load<SoundEffect>("office/fan_sound");
            fan_sound = game.Audio.Play(fanSoundEffect, volume: Character == Character.Guard ? .5f : .02f, isLooped: true);
            var lightSoundEffect = Content.Load<SoundEffect>("office/doors/light");
            light_sound = game.Audio.Play(lightSoundEffect, volume: 0f, isLooped: true, unique: true);
        }
        game.Audio.Play(game_ambience, .5f, isLooped: true);

        if(Character != Character.Guard)
        {
            CameraActive = true;
        }
        switch (Character)
        {
            case Character.Guard:
                ChangeCameraView(Guard,false);
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

        HallwayTimer = new Timer(1000);
        HallwayTimer.Elapsed += delegate
        {
            EastHall_light = (byte)rng.Next(2);
            if(ActiveView == 3 && !CameraGarble)
            {
                int index = (int)CameraData.ExtractBits(0 + (3 * ActiveView), 2 + (3 * ActiveView));
                camera_texture = CameraList[ActiveView][index+EastHall_light];
            }
        };
        HallwayTimer.AutoReset = true;
        HallwayTimer.Enabled = true;

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

        game.Client.GameAbort += Client_GameAbort;
        game.Client.GameState += Client_GameState;
        game.Client.GameMusicbox += Client_GameMusicbox;
        game.Client.RobotMove += Client_RobotMove;
        game.Client.MoveTimer += Client_MoveTimer;
        game.Client.Disconnected += Client_Disconnected;
        game.Client.JumpscareStart += Client_JumpscareStart;
        game.Client.JumpscareEnd += Client_JumpscareEnd;
        game.Client.FoxyRun += Client_FoxyRun;
        base.LoadContent();
    }

    private void Client_FoxyRun()
    {
        Debug.WriteLine("Foxy is running");
        game.Audio.Play(foxy_run);
        if (Character == Character.Guard || Character == Character.Foxy)
        {
            cam_foxy_run.Reset();
            camera_texture = cam_foxy_run[cam_foxy_run.Index];
        }
    }

    private void Client_JumpscareEnd()
    {
        ScreenManager.ReplaceScreen(new StaticScene(game));
    }

    private void Client_JumpscareStart(Character obj)
    {
        Jumpscared = obj;
        officePosition = Jumpscared == Character.Foxy ? 0 : 160;
        ToggleCamera(false);
        ActiveJumpscare = JumpscareList.First(t => t.Item1 == Jumpscared).Item2;
        ActiveJumpscare.Reset();
        robot_vocal?.Dispose();
        robot_vocal = null;
        game.Audio.Play(scream, Character == Character.Guard || Character == Jumpscared ? 1f : .3f);
    }

    private void Client_Disconnected(string obj)
    {
        ScreenManager.ReplaceScreen(new MainMenu(game));
    }

    private void Client_MoveTimer(int time)
    {
        MoveTime = time;
    }

    private void Client_RobotMove(Character character, short position)
    {
        if (character == Character.Guard)
            return;
        switch (character)
        {
            case Character.Freddy:
                Freddy = position;
                float volume = Character == Character.Guard ? .1f : .3f;
                if (Character == Character.Guard)
                {
                    volume += position switch
                    {
                        6 => .05f,
                        7 => .15f,
                        21 => .2f,
                        _ => 0
                    };
                }
                if (position == 21)
                {
                    Jumpscared = character;
                }
                else
                {
                    if(position == 9)
                    {
                        freddyMusic = game.Audio.Play(musicbox, .1f);
                    }
                    else
                    {
                        freddyMusic?.Dispose();
                    }
                }
                game.Audio.Play(freddyLaugh[rng.Next(freddyLaugh.Length)], volume);
                break;
            case Character.Bonnie:
                Bonnie = position;
                if(position == 22)
                {
                    Jumpscared = character;
                }
                break;
            case Character.Chica:
                Chica = position;
                if (position == 22)
                {
                    Jumpscared = character;
                }
                else
                {
                    if(position == 9)
                    {
                        ovenSound = game.Audio.Play(kitchenOven[rng.Next(kitchenOven.Length)], .1f);
                    }
                    else
                    {
                        ovenSound?.Dispose();
                    }
                }
                break;
            case Character.Foxy:
                if(position < Foxy)
                {
                    game.Audio.Play(knock, (30+rng.Next(50))/100f);
                }
                Foxy = position;
                if (Foxy == 5)
                {
                    Jumpscared = character;
                }
                break;
        }
        if(Character == character)
            ChangeCameraView(position);
    }

    private void Client_GameMusicbox(int state)
    {
        MusicBoxState = (byte)state;
        if (MusicBoxState == 1)
        {
            MusicBoxRunning = true;
            game.Audio.Play(musicbox, unique: true);
        }
        else if (MusicBoxState == 3)
        {
            _musicboxElapsed = 0f;
            bg_texture = officeControl[7];
            camera_texture = bg_texture;
            if (light_sound != null)
                light_sound.Volume = 0;
            game.Audio.Play(deepsteps);
        }
        else if (MusicBoxState == 4)
        {
            ScreenManager.ReplaceScreen(new FreddyScene(game));
        }
    }

    private void Client_GameState(GameState state)
    {
        if(GameState != null && Character != Character.Guard && !GameState.Camera.Active && state.Camera.Active)
            cam_rec.Reset(0);
        GameState = state;
        Time = GameState.Time;
        if (Time == 6)
        {
            ScreenManager.ReplaceScreen(new NextDay(game, game.GetScreenshot()));
            return;
        }
        Power = GameState.Power;
        CheckPower();
        BlockLeft = GameState.Left.Blocked;
        GameState.Left.Light = (!BlockLeft || !GameState.Left.Light) && GameState.Left.Light;
        BlockRight = GameState.Right.Blocked;
        GameState.Left.Light = (!BlockRight || !GameState.Right.Light) && GameState.Right.Light;
        if(GameState.Left.Light != LeftLight)
        {
            ToggleLight(true);
        }
        if(GameState.Right.Light != RightLight)
        {
            ToggleLight(false);
        }
        ToggleDoor(GameState.Left.Door, GameState.Right.Door);
        if(GameState.Camera.Garble != CameraGarble && Character == Character.Guard)
        {
            if (CameraGarble)
            {
                CameraGarble = false;
                cam_garble_sound?.Dispose();
                cam_garble_sound = null;
                ChangeCameraView(ActiveView, false);
            }
            else
            {
                PlayCameraGarble();
            }
        }
    }

    private void Client_GameAbort()
    {
        ScreenManager.ReplaceScreen(new Select(game, false));
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

        if(JumpscareRunning || !game.IsActive)
            return;

        if (CameraActive)
        {
            if (!BlockCamFlip && CamFlipStart.Contains(game.MouseState.Position) && Character == Character.Guard)
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
        if(JumpscareRunning || !game.IsActive)
            return;
        if (e.Button == MouseButton.Left)
        {
            if (CameraActive)
            {
                if(TargetView != 255 && CameraButtons[TargetView].Contains(game.MouseState.Position))
                {
                    if(Character == Character.Guard)
                        ChangeCameraView(TargetView);
                    game.Client.ChangeCameraView(TargetView);
                }
                if(Character != Character.Guard && AttackPos.Contains(game.MouseState.Position))
                    game.Client.StartAttack();
                return;
            }
            if (Clamp(NosePos).Contains(game.MouseState.Position))
            {
                game.Audio.Play(nose);
            }
            if (PowerDown) return;
            switch (ControlPanelCheck)
            {
                case 1:
                    if (lightCooldown > 0) return;
                    lightCooldown = 2;
                    if (BlockRight)
                    {
                        game.Audio.Play(error);
                        return;
                    }
                    game.Client.SetLight(false, !RightLight);
                    return;
                case 3:
                    if (lightCooldown > 0) return;
                    lightCooldown = 2;
                    if (BlockLeft)
                    {
                        game.Audio.Play(error);
                        return;
                    }
                    game.Client.SetLight(true, !LeftLight);
                    return;
                case 5:
                    if (doorCooldown > 0) return;
                    doorCooldown = 10;
                    if (BlockRight)
                    {
                        game.Audio.Play(error);
                        return;
                    }
                    game.Client.SetDoor(false, !RightDoor);
                    return;
                case 7:
                    if (doorCooldown > 0) return;
                    doorCooldown = 10;
                    if (BlockLeft)
                    {
                        game.Audio.Play(error);
                        return;
                    }
                    game.Client.SetDoor(true, !LeftDoor);
                    return;
            }
        }
    }

    private void KeyDebug(object s, KeyboardEventArgs e)
    {
        if (!game.DebugMode || JumpscareRunning)
            return;
        switch (e.Key)
        {
            case Keys.P:
                game.Client.RunCheat("power");
                break;
            case Keys.T:
                game.Client.RunCheat("time");
                break;
            case Keys.M:
                game.Client.RunCheat("move");
                break;
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
        CameraTimer?.Dispose();
        HallwayTimer?.Dispose();
        ScareTimer?.Dispose();
        StaticOpacityTimer?.Dispose();
        _keyboardListener.KeyPressed -= KeyDebug;
        _mouseListener.MouseClicked -= MouseClick;
        _mouseListener.MouseMoved -= MouseMove;
        cam_up_anim.AnimationFinished -= CamUpFinish;

        game.Client.GameAbort -= Client_GameAbort;
        game.Client.GameState -= Client_GameState;
        game.Client.GameMusicbox -= Client_GameMusicbox;
        game.Client.RobotMove -= Client_RobotMove;
        game.Client.MoveTimer -= Client_MoveTimer;
        game.Client.Disconnected -= Client_Disconnected;
        game.Client.JumpscareStart -= Client_JumpscareStart;
        game.Client.JumpscareEnd -= Client_JumpscareEnd;
        game.Client.FoxyRun -= Client_FoxyRun;
    }

    private Rectangle Clamp(Rectangle r)
    {
        r.X -= (int)officePosition;
        return r;
    }
}
