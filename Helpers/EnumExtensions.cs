using System.ComponentModel.DataAnnotations;
using System.Reflection;
using BackLog.Enums;
using BackLog.Models;

namespace BackLog.Helpers
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var member = value.GetType()
                .GetMember(value.ToString())
                .FirstOrDefault();

            var displayAttribute = member?.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.GetName() ?? value.ToString();
        }

        public static string GetPlatformIconPath(this Platform platform)
        {
            return platform switch
            {
                Platform.PC => "~/images/platforms/computer.svg.webp",
                Platform.NES => "~/images/platforms/nes.svg.webp",
                Platform.SNES => "~/images/platforms/snes.svg.webp",
                Platform.MasterSystem => "~/images/platforms/mastersystem.svg.webp",
                Platform.Genesis => "~/images/platforms/genesis.svg.webp",
                Platform.Saturn => "~/images/platforms/saturn.svg.webp",
                Platform.N64 => "~/images/platforms/n64.svg.webp",
                Platform.PS1 => "~/images/platforms/ps1.svg.webp",
                Platform.Dreamcast => "~/images/platforms/dreamcast.svg.webp",
                Platform.PS2 => "~/images/platforms/ps2.svg.webp",
                Platform.GameCube => "~/images/platforms/gamecube.svg.webp",
                Platform.Xbox => "~/images/platforms/ogxbox.svg.webp",
                Platform.Xbox360 => "~/images/platforms/xbox360.svg.webp",
                Platform.PS3 => "~/images/platforms/ps3.svg.webp",
                Platform.Wii => "~/images/platforms/wii.svg.webp",
                Platform.WiiU => "~/images/platforms/wiiu.svg.webp",
                Platform.PS4 => "~/images/platforms/ps4.svg.webp",
                Platform.XboxOne => "~/images/platforms/xboxone.svg.webp",
                Platform.Switch => "~/images/platforms/nintendoswitch.svg.webp",
                Platform.PS5 => "~/images/platforms/ps5.svg.webp",
                Platform.XboxSX => "~/images/platforms/xboxsx.svg.webp",
                Platform.Switch2 => "~/images/platforms/nintendoswitch2.svg.webp",
                Platform.GameBoy => "~/images/platforms/gameboy.svg.webp",
                Platform.GBA => "~/images/platforms/gba.svg.webp",
                Platform.DS => "~/images/platforms/ds.svg.webp",
                Platform.Nintendo3DS => "~/images/platforms/nintendo3ds.svg.webp",
                Platform.PSVita => "~/images/platforms/psvita.svg.webp",
                Platform.PSP => "~/images/platforms/psp.svg.webp",

                _ => "~/images/platforms/default.svg.webp"
            };
        }

        public static bool PlatformIconNeedsInverting(this Platform platform)
        {
            return platform switch
            {
                Platform.DS => true,
                Platform.PS3 => true,
                Platform.PS4 => true,
                Platform.PS5 => true,
                Platform.PSP => true,
                Platform.XboxSX => true,
                Platform.Switch => true,
                _ => GetPlatformIconPath(platform) == "~/images/platforms/default.svg.webp"
            };
        }

        public static string GetMusicFormatIconPath(this Format format)
        {
            return format switch
            {
                Format.Vinyl => "~/images/music-formats/vinyl.svg.webp",
                Format.CD => "~/images/music-formats/cd.svg.webp",
                Format.Cassette => "~/images/music-formats/cassette.svg.webp",
                Format.Digital => "~/images/music-formats/digital.svg.webp",
                _ => ""
            };
        }

        public static string GetBookFormatIconPath(this BookFormat format)
        {
            return format switch
            {
                BookFormat.Hardcover => "~/images/book-formats/hardcover.svg.webp",
                BookFormat.Softcover => "~/images/book-formats/softcover.svg.webp",
                BookFormat.Digital => "~/images/book-formats/digital.svg.webp",
                _ => ""
            };
        }

        public static string GetGameStatusIconPath(this Status status)
        {
            return status switch
            {
                Status.FullyCompleted => "~/images/statuses/100%.webp",
                Status.Completed => "~/images/statuses/complete.webp",
                Status.Started => "~/images/statuses/incomplete.webp",
                Status.NotStarted => "~/images/statuses/nostart.webp",
                Status.Abandoned => "~/images/statuses/abandoned.webp",
                _ => ""
            };
        }

        public static string GetStatusIconPath(this CompletionStatus status)
        {
            return status switch
            {
                CompletionStatus.Completed => "~/images/statuses/complete.webp",
                CompletionStatus.Incomplete => "~/images/statuses/incomplete.webp",
                CompletionStatus.NotStarted => "~/images/statuses/nostart.webp",
                CompletionStatus.Abandoned => "~/images/statuses/abandoned.webp",
                _ => ""
            };
        }


    }


}
