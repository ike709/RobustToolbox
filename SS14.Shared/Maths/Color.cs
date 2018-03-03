﻿//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2008 the Open Toolkit library, except where noted.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using SysColor = System.Drawing.Color;

namespace SS14.Shared.Maths
{
    /// <summary>
    ///     Represents a color with 4 floating-point components (R, G, B, A).
    /// </summary>
    [Serializable]
    public struct Color : IEquatable<Color>
    {
        /// <summary>
        ///     The red component of this Color4 structure.
        /// </summary>
        public readonly float R;

        /// <summary>
        ///     The green component of this Color4 structure.
        /// </summary>
        public readonly float G;

        /// <summary>
        ///     The blue component of this Color4 structure.
        /// </summary>
        public readonly float B;

        /// <summary>
        ///     The alpha component of this Color4 structure.
        /// </summary>
        public readonly float A;

        public byte RByte => (byte) (R * byte.MaxValue);
        public byte GByte => (byte) (G * byte.MaxValue);
        public byte BByte => (byte) (B * byte.MaxValue);
        public byte AByte => (byte) (A * byte.MaxValue);

        /// <summary>
        ///     Constructs a new Color4 structure from the specified components.
        /// </summary>
        /// <param name="r">The red component of the new Color4 structure.</param>
        /// <param name="g">The green component of the new Color4 structure.</param>
        /// <param name="b">The blue component of the new Color4 structure.</param>
        /// <param name="a">The alpha component of the new Color4 structure.</param>
        public Color(float r, float g, float b, float a = 1)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        ///     Constructs a new Color4 structure from the specified components.
        /// </summary>
        /// <param name="r">The red component of the new Color4 structure.</param>
        /// <param name="g">The green component of the new Color4 structure.</param>
        /// <param name="b">The blue component of the new Color4 structure.</param>
        /// <param name="a">The alpha component of the new Color4 structure.</param>
        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r / (float) byte.MaxValue;
            G = g / (float) byte.MaxValue;
            B = b / (float) byte.MaxValue;
            A = a / (float) byte.MaxValue;
        }

        /// <summary>
        ///     Converts this color to an integer representation with 8 bits per channel.
        /// </summary>
        /// <returns>A <see cref="System.Int32" /> that represents this instance.</returns>
        /// <remarks>
        ///     This method is intended only for compatibility with System.Drawing. It compresses the color into 8 bits per
        ///     channel, which means color information is lost.
        /// </remarks>
        public int ToArgb()
        {
            var value =
                ((uint) (A * byte.MaxValue) << 24) |
                ((uint) (R * byte.MaxValue) << 16) |
                ((uint) (G * byte.MaxValue) << 8) |
                (uint) (B * byte.MaxValue);

            return unchecked((int) value);
        }

        /// <summary>
        ///     Compares the specified Color4 structures for equality.
        /// </summary>
        /// <param name="left">The left-hand side of the comparison.</param>
        /// <param name="right">The right-hand side of the comparison.</param>
        /// <returns>True if left is equal to right; false otherwise.</returns>
        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Compares the specified Color4 structures for inequality.
        /// </summary>
        /// <param name="left">The left-hand side of the comparison.</param>
        /// <param name="right">The right-hand side of the comparison.</param>
        /// <returns>True if left is not equal to right; false otherwise.</returns>
        public static bool operator !=(Color left, Color right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Converts the specified System.Drawing.Color to a Color4 structure.
        /// </summary>
        /// <param name="color">The System.Drawing.Color to convert.</param>
        /// <returns>A new Color4 structure containing the converted components.</returns>
        public static implicit operator Color(SysColor color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        ///     Converts the specified Color4 to a System.Drawing.Color structure.
        /// </summary>
        /// <param name="color">The Color4 to convert.</param>
        /// <returns>A new System.Drawing.Color structure containing the converted components.</returns>
        public static explicit operator SysColor(Color color)
        {
            return SysColor.FromArgb(
                (int) (color.A * byte.MaxValue),
                (int) (color.R * byte.MaxValue),
                (int) (color.G * byte.MaxValue),
                (int) (color.B * byte.MaxValue));
        }

        /// <summary>
        ///     Compares whether this Color4 structure is equal to the specified object.
        /// </summary>
        /// <param name="obj">An object to compare to.</param>
        /// <returns>True obj is a Color4 structure with the same components as this Color4; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Color))
                return false;

            return Equals((Color) obj);
        }

        /// <summary>
        ///     Calculates the hash code for this Color4 structure.
        /// </summary>
        /// <returns>A System.Int32 containing the hash code of this Color4 structure.</returns>
        public override int GetHashCode()
        {
            return ToArgb();
        }

        /// <summary>
        ///     Creates a System.String that describes this Color4 structure.
        /// </summary>
        /// <returns>A System.String that describes this Color4 structure.</returns>
        public override string ToString()
        {
            return $"{{(R, G, B, A) = ({R}, {G}, {B}, {A})}}";
        }

        #region Static Colors

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 255, 255, 0).
        /// </summary>
        public static Color Transparent => new Color(255, 255, 255, 0);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (240, 248, 255, 255).
        /// </summary>
        public static Color AliceBlue => new Color(240, 248, 255, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (250, 235, 215, 255).
        /// </summary>
        public static Color AntiqueWhite => new Color(250, 235, 215, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 255, 255, 255).
        /// </summary>
        public static Color Aqua => new Color(0, 255, 255, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (127, 255, 212, 255).
        /// </summary>
        public static Color Aquamarine => new Color(127, 255, 212, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (240, 255, 255, 255).
        /// </summary>
        public static Color Azure => new Color(240, 255, 255, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (245, 245, 220, 255).
        /// </summary>
        public static Color Beige => new Color(245, 245, 220, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 228, 196, 255).
        /// </summary>
        public static Color Bisque => new Color(255, 228, 196, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 0, 0, 255).
        /// </summary>
        public static Color Black => new Color(0, 0, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 235, 205, 255).
        /// </summary>
        public static Color BlanchedAlmond => new Color(255, 235, 205, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 0, 255, 255).
        /// </summary>
        public static Color Blue => new Color(0, 0, 255, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (138, 43, 226, 255).
        /// </summary>
        public static Color BlueViolet => new Color(138, 43, 226, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (165, 42, 42, 255).
        /// </summary>
        public static Color Brown => new Color(165, 42, 42, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (222, 184, 135, 255).
        /// </summary>
        public static Color BurlyWood => new Color(222, 184, 135, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (95, 158, 160, 255).
        /// </summary>
        public static Color CadetBlue => new Color(95, 158, 160, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (127, 255, 0, 255).
        /// </summary>
        public static Color Chartreuse => new Color(127, 255, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (210, 105, 30, 255).
        /// </summary>
        public static Color Chocolate => new Color(210, 105, 30, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 127, 80, 255).
        /// </summary>
        public static Color Coral => new Color(255, 127, 80, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (100, 149, 237, 255).
        /// </summary>
        public static Color CornflowerBlue => new Color(100, 149, 237, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 248, 220, 255).
        /// </summary>
        public static Color Cornsilk => new Color(255, 248, 220, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (220, 20, 60, 255).
        /// </summary>
        public static Color Crimson => new Color(220, 20, 60, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 255, 255, 255).
        /// </summary>
        public static Color Cyan => new Color(0, 255, 255, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 0, 139, 255).
        /// </summary>
        public static Color DarkBlue => new Color(0, 0, 139, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 139, 139, 255).
        /// </summary>
        public static Color DarkCyan => new Color(0, 139, 139, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (184, 134, 11, 255).
        /// </summary>
        public static Color DarkGoldenrod => new Color(184, 134, 11, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (169, 169, 169, 255).
        /// </summary>
        public static Color DarkGray => new Color(169, 169, 169, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 100, 0, 255).
        /// </summary>
        public static Color DarkGreen => new Color(0, 100, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (189, 183, 107, 255).
        /// </summary>
        public static Color DarkKhaki => new Color(189, 183, 107, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (139, 0, 139, 255).
        /// </summary>
        public static Color DarkMagenta => new Color(139, 0, 139, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (85, 107, 47, 255).
        /// </summary>
        public static Color DarkOliveGreen => new Color(85, 107, 47, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 140, 0, 255).
        /// </summary>
        public static Color DarkOrange => new Color(255, 140, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (153, 50, 204, 255).
        /// </summary>
        public static Color DarkOrchid => new Color(153, 50, 204, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (139, 0, 0, 255).
        /// </summary>
        public static Color DarkRed => new Color(139, 0, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (233, 150, 122, 255).
        /// </summary>
        public static Color DarkSalmon => new Color(233, 150, 122, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (143, 188, 139, 255).
        /// </summary>
        public static Color DarkSeaGreen => new Color(143, 188, 139, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (72, 61, 139, 255).
        /// </summary>
        public static Color DarkSlateBlue => new Color(72, 61, 139, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (47, 79, 79, 255).
        /// </summary>
        public static Color DarkSlateGray => new Color(47, 79, 79, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 206, 209, 255).
        /// </summary>
        public static Color DarkTurquoise => new Color(0, 206, 209, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (148, 0, 211, 255).
        /// </summary>
        public static Color DarkViolet => new Color(148, 0, 211, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 20, 147, 255).
        /// </summary>
        public static Color DeepPink => new Color(255, 20, 147, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 191, 255, 255).
        /// </summary>
        public static Color DeepSkyBlue => new Color(0, 191, 255, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (105, 105, 105, 255).
        /// </summary>
        public static Color DimGray => new Color(105, 105, 105, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (30, 144, 255, 255).
        /// </summary>
        public static Color DodgerBlue => new Color(30, 144, 255, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (178, 34, 34, 255).
        /// </summary>
        public static Color Firebrick => new Color(178, 34, 34, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 250, 240, 255).
        /// </summary>
        public static Color FloralWhite => new Color(255, 250, 240, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (34, 139, 34, 255).
        /// </summary>
        public static Color ForestGreen => new Color(34, 139, 34, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 0, 255, 255).
        /// </summary>
        public static Color Fuchsia => new Color(255, 0, 255, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (220, 220, 220, 255).
        /// </summary>
        public static Color Gainsboro => new Color(220, 220, 220, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (248, 248, 255, 255).
        /// </summary>
        public static Color GhostWhite => new Color(248, 248, 255, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 215, 0, 255).
        /// </summary>
        public static Color Gold => new Color(255, 215, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (218, 165, 32, 255).
        /// </summary>
        public static Color Goldenrod => new Color(218, 165, 32, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (128, 128, 128, 255).
        /// </summary>
        public static Color Gray => new Color(128, 128, 128, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 128, 0, 255).
        /// </summary>
        public static Color Green => new Color(0, 128, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (173, 255, 47, 255).
        /// </summary>
        public static Color GreenYellow => new Color(173, 255, 47, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (240, 255, 240, 255).
        /// </summary>
        public static Color Honeydew => new Color(240, 255, 240, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 105, 180, 255).
        /// </summary>
        public static Color HotPink => new Color(255, 105, 180, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (205, 92, 92, 255).
        /// </summary>
        public static Color IndianRed => new Color(205, 92, 92, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (75, 0, 130, 255).
        /// </summary>
        public static Color Indigo => new Color(75, 0, 130, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 255, 240, 255).
        /// </summary>
        public static Color Ivory => new Color(255, 255, 240, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (240, 230, 140, 255).
        /// </summary>
        public static Color Khaki => new Color(240, 230, 140, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (230, 230, 250, 255).
        /// </summary>
        public static Color Lavender => new Color(230, 230, 250, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 240, 245, 255).
        /// </summary>
        public static Color LavenderBlush => new Color(255, 240, 245, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (124, 252, 0, 255).
        /// </summary>
        public static Color LawnGreen => new Color(124, 252, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 250, 205, 255).
        /// </summary>
        public static Color LemonChiffon => new Color(255, 250, 205, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (173, 216, 230, 255).
        /// </summary>
        public static Color LightBlue => new Color(173, 216, 230, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (240, 128, 128, 255).
        /// </summary>
        public static Color LightCoral => new Color(240, 128, 128, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (224, 255, 255, 255).
        /// </summary>
        public static Color LightCyan => new Color(224, 255, 255, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (250, 250, 210, 255).
        /// </summary>
        public static Color LightGoldenrodYellow => new Color(250, 250, 210, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (144, 238, 144, 255).
        /// </summary>
        public static Color LightGreen => new Color(144, 238, 144, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (211, 211, 211, 255).
        /// </summary>
        public static Color LightGray => new Color(211, 211, 211, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 182, 193, 255).
        /// </summary>
        public static Color LightPink => new Color(255, 182, 193, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 160, 122, 255).
        /// </summary>
        public static Color LightSalmon => new Color(255, 160, 122, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (32, 178, 170, 255).
        /// </summary>
        public static Color LightSeaGreen => new Color(32, 178, 170, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (135, 206, 250, 255).
        /// </summary>
        public static Color LightSkyBlue => new Color(135, 206, 250, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (119, 136, 153, 255).
        /// </summary>
        public static Color LightSlateGray => new Color(119, 136, 153, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (176, 196, 222, 255).
        /// </summary>
        public static Color LightSteelBlue => new Color(176, 196, 222, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 255, 224, 255).
        /// </summary>
        public static Color LightYellow => new Color(255, 255, 224, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 255, 0, 255).
        /// </summary>
        public static Color Lime => new Color(0, 255, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (50, 205, 50, 255).
        /// </summary>
        public static Color LimeGreen => new Color(50, 205, 50, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (250, 240, 230, 255).
        /// </summary>
        public static Color Linen => new Color(250, 240, 230, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 0, 255, 255).
        /// </summary>
        public static Color Magenta => new Color(255, 0, 255, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (128, 0, 0, 255).
        /// </summary>
        public static Color Maroon => new Color(128, 0, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (102, 205, 170, 255).
        /// </summary>
        public static Color MediumAquamarine => new Color(102, 205, 170, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 0, 205, 255).
        /// </summary>
        public static Color MediumBlue => new Color(0, 0, 205, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (186, 85, 211, 255).
        /// </summary>
        public static Color MediumOrchid => new Color(186, 85, 211, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (147, 112, 219, 255).
        /// </summary>
        public static Color MediumPurple => new Color(147, 112, 219, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (60, 179, 113, 255).
        /// </summary>
        public static Color MediumSeaGreen => new Color(60, 179, 113, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (123, 104, 238, 255).
        /// </summary>
        public static Color MediumSlateBlue => new Color(123, 104, 238, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 250, 154, 255).
        /// </summary>
        public static Color MediumSpringGreen => new Color(0, 250, 154, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (72, 209, 204, 255).
        /// </summary>
        public static Color MediumTurquoise => new Color(72, 209, 204, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (199, 21, 133, 255).
        /// </summary>
        public static Color MediumVioletRed => new Color(199, 21, 133, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (25, 25, 112, 255).
        /// </summary>
        public static Color MidnightBlue => new Color(25, 25, 112, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (245, 255, 250, 255).
        /// </summary>
        public static Color MintCream => new Color(245, 255, 250, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 228, 225, 255).
        /// </summary>
        public static Color MistyRose => new Color(255, 228, 225, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 228, 181, 255).
        /// </summary>
        public static Color Moccasin => new Color(255, 228, 181, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 222, 173, 255).
        /// </summary>
        public static Color NavajoWhite => new Color(255, 222, 173, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 0, 128, 255).
        /// </summary>
        public static Color Navy => new Color(0, 0, 128, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (253, 245, 230, 255).
        /// </summary>
        public static Color OldLace => new Color(253, 245, 230, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (128, 128, 0, 255).
        /// </summary>
        public static Color Olive => new Color(128, 128, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (107, 142, 35, 255).
        /// </summary>
        public static Color OliveDrab => new Color(107, 142, 35, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 165, 0, 255).
        /// </summary>
        public static Color Orange => new Color(255, 165, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 69, 0, 255).
        /// </summary>
        public static Color OrangeRed => new Color(255, 69, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (218, 112, 214, 255).
        /// </summary>
        public static Color Orchid => new Color(218, 112, 214, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (238, 232, 170, 255).
        /// </summary>
        public static Color PaleGoldenrod => new Color(238, 232, 170, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (152, 251, 152, 255).
        /// </summary>
        public static Color PaleGreen => new Color(152, 251, 152, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (175, 238, 238, 255).
        /// </summary>
        public static Color PaleTurquoise => new Color(175, 238, 238, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (219, 112, 147, 255).
        /// </summary>
        public static Color PaleVioletRed => new Color(219, 112, 147, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 239, 213, 255).
        /// </summary>
        public static Color PapayaWhip => new Color(255, 239, 213, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 218, 185, 255).
        /// </summary>
        public static Color PeachPuff => new Color(255, 218, 185, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (205, 133, 63, 255).
        /// </summary>
        public static Color Peru => new Color(205, 133, 63, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 192, 203, 255).
        /// </summary>
        public static Color Pink => new Color(255, 192, 203, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (221, 160, 221, 255).
        /// </summary>
        public static Color Plum => new Color(221, 160, 221, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (176, 224, 230, 255).
        /// </summary>
        public static Color PowderBlue => new Color(176, 224, 230, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (128, 0, 128, 255).
        /// </summary>
        public static Color Purple => new Color(128, 0, 128, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 0, 0, 255).
        /// </summary>
        public static Color Red => new Color(255, 0, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (188, 143, 143, 255).
        /// </summary>
        public static Color RosyBrown => new Color(188, 143, 143, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (65, 105, 225, 255).
        /// </summary>
        public static Color RoyalBlue => new Color(65, 105, 225, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (139, 69, 19, 255).
        /// </summary>
        public static Color SaddleBrown => new Color(139, 69, 19, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (250, 128, 114, 255).
        /// </summary>
        public static Color Salmon => new Color(250, 128, 114, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (244, 164, 96, 255).
        /// </summary>
        public static Color SandyBrown => new Color(244, 164, 96, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (46, 139, 87, 255).
        /// </summary>
        public static Color SeaGreen => new Color(46, 139, 87, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 245, 238, 255).
        /// </summary>
        public static Color SeaShell => new Color(255, 245, 238, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (160, 82, 45, 255).
        /// </summary>
        public static Color Sienna => new Color(160, 82, 45, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (192, 192, 192, 255).
        /// </summary>
        public static Color Silver => new Color(192, 192, 192, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (135, 206, 235, 255).
        /// </summary>
        public static Color SkyBlue => new Color(135, 206, 235, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (106, 90, 205, 255).
        /// </summary>
        public static Color SlateBlue => new Color(106, 90, 205, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (112, 128, 144, 255).
        /// </summary>
        public static Color SlateGray => new Color(112, 128, 144, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 250, 250, 255).
        /// </summary>
        public static Color Snow => new Color(255, 250, 250, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 255, 127, 255).
        /// </summary>
        public static Color SpringGreen => new Color(0, 255, 127, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (70, 130, 180, 255).
        /// </summary>
        public static Color SteelBlue => new Color(70, 130, 180, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (210, 180, 140, 255).
        /// </summary>
        public static Color Tan => new Color(210, 180, 140, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (0, 128, 128, 255).
        /// </summary>
        public static Color Teal => new Color(0, 128, 128, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (216, 191, 216, 255).
        /// </summary>
        public static Color Thistle => new Color(216, 191, 216, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 99, 71, 255).
        /// </summary>
        public static Color Tomato => new Color(255, 99, 71, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (64, 224, 208, 255).
        /// </summary>
        public static Color Turquoise => new Color(64, 224, 208, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (238, 130, 238, 255).
        /// </summary>
        public static Color Violet => new Color(238, 130, 238, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (245, 222, 179, 255).
        /// </summary>
        public static Color Wheat => new Color(245, 222, 179, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 255, 255, 255).
        /// </summary>
        public static Color White => new Color(255, 255, 255, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (245, 245, 245, 255).
        /// </summary>
        public static Color WhiteSmoke => new Color(245, 245, 245, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (255, 255, 0, 255).
        /// </summary>
        public static Color Yellow => new Color(255, 255, 0, 255);

        /// <summary>
        ///     Gets the system color with (R, G, B, A) = (154, 205, 50, 255).
        /// </summary>
        public static Color YellowGreen => new Color(154, 205, 50, 255);

        #endregion

        public Color WithRed(float newR)
        {
            return new Color(newR, G, B, A);
        }

        public Color WithGreen(float newG)
        {
            return new Color(R, newG, B, A);
        }

        public Color WithBlue(float newB)
        {
            return new Color(R, G, newB, A);
        }

        public Color WithAlpha(float newA)
        {
            return new Color(R, G, B, newA);
        }

        public Color WithRed(byte newR)
        {
            return new Color((float) newR / byte.MaxValue, G, B, A);
        }

        public Color WithGreen(byte newG)
        {
            return new Color(R, (float) newG / byte.MaxValue, B, A);
        }

        public Color WithBlue(byte newB)
        {
            return new Color(R, G, (float) newB / byte.MaxValue, A);
        }

        public Color WithAlpha(byte newA)
        {
            return new Color(R, G, B, (float) newA / byte.MaxValue);
        }

        /// <summary>
        ///     Converts sRGB color values to RGB color values.
        /// </summary>
        /// <returns>
        ///     Returns the converted color value.
        /// </returns>
        /// <param name="srgb">
        ///     Color value to convert in sRGB.
        /// </param>
        public static Color FromSrgb(Color srgb)
        {
            float r, g, b;

            if (srgb.R <= 0.04045f)
                r = srgb.R / 12.92f;
            else
                r = (float) Math.Pow((srgb.R + 0.055f) / (1.0f + 0.055f), 2.4f);

            if (srgb.G <= 0.04045f)
                g = srgb.G / 12.92f;
            else
                g = (float) Math.Pow((srgb.G + 0.055f) / (1.0f + 0.055f), 2.4f);

            if (srgb.B <= 0.04045f)
                b = srgb.B / 12.92f;
            else
                b = (float) Math.Pow((srgb.B + 0.055f) / (1.0f + 0.055f), 2.4f);

            return new Color(r, g, b, srgb.A);
        }

        /// <summary>
        ///     Converts RGB color values to sRGB color values.
        /// </summary>
        /// <returns>
        ///     Returns the converted color value.
        /// </returns>
        /// <param name="rgb">Color value to convert.</param>
        public static Color ToSrgb(Color rgb)
        {
            float r, g, b;

            if (rgb.R <= 0.0031308)
                r = 12.92f * rgb.R;
            else
                r = (1.0f + 0.055f) * (float) Math.Pow(rgb.R, 1.0f / 2.4f) - 0.055f;

            if (rgb.G <= 0.0031308)
                g = 12.92f * rgb.G;
            else
                g = (1.0f + 0.055f) * (float) Math.Pow(rgb.G, 1.0f / 2.4f) - 0.055f;

            if (rgb.B <= 0.0031308)
                b = 12.92f * rgb.B;
            else
                b = (1.0f + 0.055f) * (float) Math.Pow(rgb.B, 1.0f / 2.4f) - 0.055f;

            return new Color(r, g, b, rgb.A);
        }

        /// <summary>
        ///     Converts HSL color values to RGB color values.
        /// </summary>
        /// <returns>
        ///     Returns the converted color value.
        /// </returns>
        /// <param name="hsl">
        ///     Color value to convert in hue, saturation, lightness (HSL).
        ///     The X element is Hue (H), the Y element is Saturation (S), the Z element is Lightness (L), and the W element is
        ///     Alpha (which is copied to the output's Alpha value).
        ///     Each has a range of 0.0 to 1.0.
        /// </param>
        public static Color FromHsl(Vector4 hsl)
        {
            var hue = hsl.X * 360.0f;
            var saturation = hsl.Y;
            var lightness = hsl.Z;

            var c = (1.0f - Math.Abs(2.0f * lightness - 1.0f)) * saturation;

            var h = hue / 60.0f;
            var X = c * (1.0f - Math.Abs(h % 2.0f - 1.0f));

            float r, g, b;
            if (0.0f <= h && h < 1.0f)
            {
                r = c;
                g = X;
                b = 0.0f;
            }
            else if (1.0f <= h && h < 2.0f)
            {
                r = X;
                g = c;
                b = 0.0f;
            }
            else if (2.0f <= h && h < 3.0f)
            {
                r = 0.0f;
                g = c;
                b = X;
            }
            else if (3.0f <= h && h < 4.0f)
            {
                r = 0.0f;
                g = X;
                b = c;
            }
            else if (4.0f <= h && h < 5.0f)
            {
                r = X;
                g = 0.0f;
                b = c;
            }
            else if (5.0f <= h && h < 6.0f)
            {
                r = c;
                g = 0.0f;
                b = X;
            }
            else
            {
                r = 0.0f;
                g = 0.0f;
                b = 0.0f;
            }

            var m = lightness - c / 2.0f;
            return new Color(r + m, g + m, b + m, hsl.W);
        }

        /// <summary>
        ///     Converts RGB color values to HSL color values.
        /// </summary>
        /// <returns>
        ///     Returns the converted color value.
        ///     The X element is Hue (H), the Y element is Saturation (S), the Z element is Lightness (L), and the W element is
        ///     Alpha (a copy of the input's Alpha value).
        ///     Each has a range of 0.0 to 1.0.
        /// </returns>
        /// <param name="rgb">Color value to convert.</param>
        public static Vector4 ToHsl(Color rgb)
        {
            var max = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
            var min = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));
            var c = max - min;

            var h = 0.0f;
            if (max == rgb.R)
                h = (rgb.G - rgb.B) / c;
            else if (max == rgb.G)
                h = (rgb.B - rgb.R) / c + 2.0f;
            else if (max == rgb.B)
                h = (rgb.R - rgb.G) / c + 4.0f;

            var hue = h / 6.0f;
            if (hue < 0.0f)
                hue += 1.0f;

            var lightness = (max + min) / 2.0f;

            var saturation = 0.0f;
            if (0.0f != lightness && lightness != 1.0f)
                saturation = c / (1.0f - Math.Abs(2.0f * lightness - 1.0f));

            return new Vector4(hue, saturation, lightness, rgb.A);
        }

        /// <summary>
        ///     Converts HSV color values to RGB color values.
        /// </summary>
        /// <returns>
        ///     Returns the converted color value.
        /// </returns>
        /// <param name="hsv">
        ///     Color value to convert in hue, saturation, value (HSV).
        ///     The X element is Hue (H), the Y element is Saturation (S), the Z element is Value (V), and the W element is Alpha
        ///     (which is copied to the output's Alpha value).
        ///     Each has a range of 0.0 to 1.0.
        /// </param>
        public static Color FromHsv(Vector4 hsv)
        {
            var hue = hsv.X * 360.0f;
            var saturation = hsv.Y;
            var value = hsv.Z;

            var c = value * saturation;

            var h = hue / 60.0f;
            var x = c * (1.0f - Math.Abs(h % 2.0f - 1.0f));

            float r, g, b;
            if (0.0f <= h && h < 1.0f)
            {
                r = c;
                g = x;
                b = 0.0f;
            }
            else if (1.0f <= h && h < 2.0f)
            {
                r = x;
                g = c;
                b = 0.0f;
            }
            else if (2.0f <= h && h < 3.0f)
            {
                r = 0.0f;
                g = c;
                b = x;
            }
            else if (3.0f <= h && h < 4.0f)
            {
                r = 0.0f;
                g = x;
                b = c;
            }
            else if (4.0f <= h && h < 5.0f)
            {
                r = x;
                g = 0.0f;
                b = c;
            }
            else if (5.0f <= h && h < 6.0f)
            {
                r = c;
                g = 0.0f;
                b = x;
            }
            else
            {
                r = 0.0f;
                g = 0.0f;
                b = 0.0f;
            }

            var m = value - c;
            return new Color(r + m, g + m, b + m, hsv.W);
        }

        /// <summary>
        ///     Converts RGB color values to HSV color values.
        /// </summary>
        /// <returns>
        ///     Returns the converted color value.
        ///     The X element is Hue (H), the Y element is Saturation (S), the Z element is Value (V), and the W element is Alpha
        ///     (a copy of the input's Alpha value).
        ///     Each has a range of 0.0 to 1.0.
        /// </returns>
        /// <param name="rgb">Color value to convert.</param>
        public static Vector4 ToHsv(Color rgb)
        {
            var max = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
            var min = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));
            var c = max - min;

            var h = 0.0f;
            if (max == rgb.R)
                h = (rgb.G - rgb.B) / c % 6.0f;
            else if (max == rgb.G)
                h = (rgb.B - rgb.R) / c + 2.0f;
            else if (max == rgb.B)
                h = (rgb.R - rgb.G) / c + 4.0f;

            var hue = h * 60.0f / 360.0f;

            var saturation = 0.0f;
            if (0.0f != max)
                saturation = c / max;

            return new Vector4(hue, saturation, max, rgb.A);
        }

        /// <summary>
        ///     Converts XYZ color values to RGB color values.
        /// </summary>
        /// <returns>
        ///     Returns the converted color value.
        /// </returns>
        /// <param name="xyz">
        ///     Color value to convert with the trisimulus values of X, Y, and Z in the corresponding element, and the W element
        ///     with Alpha (which is copied to the output's Alpha value).
        ///     Each has a range of 0.0 to 1.0.
        /// </param>
        /// <remarks>Uses the CIE XYZ colorspace.</remarks>
        public static Color FromXyz(Vector4 xyz)
        {
            var r = 0.41847f * xyz.X + -0.15866f * xyz.Y + -0.082835f * xyz.Z;
            var g = -0.091169f * xyz.X + 0.25243f * xyz.Y + 0.015708f * xyz.Z;
            var b = 0.00092090f * xyz.X + -0.0025498f * xyz.Y + 0.17860f * xyz.Z;
            return new Color(r, g, b, xyz.W);
        }

        /// <summary>
        ///     Converts RGB color values to XYZ color values.
        /// </summary>
        /// <returns>
        ///     Returns the converted color value with the trisimulus values of X, Y, and Z in the corresponding element, and the W
        ///     element with Alpha (a copy of the input's Alpha value).
        ///     Each has a range of 0.0 to 1.0.
        /// </returns>
        /// <param name="rgb">Color value to convert.</param>
        /// <remarks>Uses the CIE XYZ colorspace.</remarks>
        public static Vector4 ToXyz(Color rgb)
        {
            var x = (0.49f * rgb.R + 0.31f * rgb.G + 0.20f * rgb.B) / 0.17697f;
            var y = (0.17697f * rgb.R + 0.81240f * rgb.G + 0.01063f * rgb.B) / 0.17697f;
            var z = (0.00f * rgb.R + 0.01f * rgb.G + 0.99f * rgb.B) / 0.17697f;
            return new Vector4(x, y, z, rgb.A);
        }

        /// <summary>
        ///     Converts YCbCr color values to RGB color values.
        /// </summary>
        /// <returns>
        ///     Returns the converted color value.
        /// </returns>
        /// <param name="ycbcr">
        ///     Color value to convert in Luma-Chrominance (YCbCr) aka YUV.
        ///     The X element contains Luma (Y, 0.0 to 1.0), the Y element contains Blue-difference chroma (U, -0.5 to 0.5), the Z
        ///     element contains the Red-difference chroma (V, -0.5 to 0.5), and the W element contains the Alpha (which is copied
        ///     to the output's Alpha value).
        /// </param>
        /// <remarks>Converts using ITU-R BT.601/CCIR 601 W(r) = 0.299 W(b) = 0.114 U(max) = 0.436 V(max) = 0.615.</remarks>
        public static Color FromYcbcr(Vector4 ycbcr)
        {
            var r = 1.0f * ycbcr.X + 0.0f * ycbcr.Y + 1.402f * ycbcr.Z;
            var g = 1.0f * ycbcr.X + -0.344136f * ycbcr.Y + -0.714136f * ycbcr.Z;
            var b = 1.0f * ycbcr.X + 1.772f * ycbcr.Y + 0.0f * ycbcr.Z;
            return new Color(r, g, b, ycbcr.W);
        }

        /// <summary>
        ///     Converts RGB color values to YUV color values.
        /// </summary>
        /// <returns>
        ///     Returns the converted color value in Luma-Chrominance (YCbCr) aka YUV.
        ///     The X element contains Luma (Y, 0.0 to 1.0), the Y element contains Blue-difference chroma (U, -0.5 to 0.5), the Z
        ///     element contains the Red-difference chroma (V, -0.5 to 0.5), and the W element contains the Alpha (a copy of the
        ///     input's Alpha value).
        ///     Each has a range of 0.0 to 1.0.
        /// </returns>
        /// <param name="rgb">Color value to convert.</param>
        /// <remarks>Converts using ITU-R BT.601/CCIR 601 W(r) = 0.299 W(b) = 0.114 U(max) = 0.436 V(max) = 0.615.</remarks>
        public static Vector4 ToYcbcr(Color rgb)
        {
            var y = 0.299f * rgb.R + 0.587f * rgb.G + 0.114f * rgb.B;
            var u = -0.168736f * rgb.R + -0.331264f * rgb.G + 0.5f * rgb.B;
            var v = 0.5f * rgb.R + -0.418688f * rgb.G + -0.081312f * rgb.B;
            return new Vector4(y, u, v, rgb.A);
        }

        /// <summary>
        ///     Converts HCY color values to RGB color values.
        /// </summary>
        /// <returns>
        ///     Returns the converted color value.
        /// </returns>
        /// <param name="hcy">
        ///     Color value to convert in hue, chroma, luminance (HCY).
        ///     The X element is Hue (H), the Y element is Chroma (C), the Z element is luminance (Y), and the W element is Alpha
        ///     (which is copied to the output's Alpha value).
        ///     Each has a range of 0.0 to 1.0.
        /// </param>
        public static Color FromHcy(Vector4 hcy)
        {
            var hue = hcy.X * 360.0f;
            var c = hcy.Y;
            var luminance = hcy.Z;

            var h = hue / 60.0f;
            var x = c * (1.0f - Math.Abs(h % 2.0f - 1.0f));

            float r, g, b;
            if (0.0f <= h && h < 1.0f)
            {
                r = c;
                g = x;
                b = 0.0f;
            }
            else if (1.0f <= h && h < 2.0f)
            {
                r = x;
                g = c;
                b = 0.0f;
            }
            else if (2.0f <= h && h < 3.0f)
            {
                r = 0.0f;
                g = c;
                b = x;
            }
            else if (3.0f <= h && h < 4.0f)
            {
                r = 0.0f;
                g = x;
                b = c;
            }
            else if (4.0f <= h && h < 5.0f)
            {
                r = x;
                g = 0.0f;
                b = c;
            }
            else if (5.0f <= h && h < 6.0f)
            {
                r = c;
                g = 0.0f;
                b = x;
            }
            else
            {
                r = 0.0f;
                g = 0.0f;
                b = 0.0f;
            }

            var m = luminance - (0.30f * r + 0.59f * g + 0.11f * b);
            return new Color(r + m, g + m, b + m, hcy.W);
        }

        /// <summary>
        ///     Converts RGB color values to HCY color values.
        /// </summary>
        /// <returns>
        ///     Returns the converted color value.
        ///     The X element is Hue (H), the Y element is Chroma (C), the Z element is luminance (Y), and the W element is Alpha
        ///     (a copy of the input's Alpha value).
        ///     Each has a range of 0.0 to 1.0.
        /// </returns>
        /// <param name="rgb">Color value to convert.</param>
        public static Vector4 ToHcy(Color rgb)
        {
            var max = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
            var min = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));
            var c = max - min;

            var h = 0.0f;
            if (max == rgb.R)
                h = (rgb.G - rgb.B) / c % 6.0f;
            else if (max == rgb.G)
                h = (rgb.B - rgb.R) / c + 2.0f;
            else if (max == rgb.B)
                h = (rgb.R - rgb.G) / c + 4.0f;

            var hue = h * 60.0f / 360.0f;

            var luminance = 0.30f * rgb.R + 0.59f * rgb.G + 0.11f * rgb.B;

            return new Vector4(hue, c, luminance, rgb.A);
        }

        /// <summary>
        ///     Interpolate two colors with a lambda, AKA returning the two colors combined with a ratio of
        ///     <paramref name="lambda" />.
        /// </summary>
        /// <param name="endPoint1"></param>
        /// <param name="endPoint2"></param>
        /// <param name="lambda">
        ///     A value ranging from 0-1. The higher the value the more is taken from <paramref name="endPoint1" />,
        ///     with 0.5 being 50% of both colors, 0.25 being 25% of <paramref name="endPoint1" /> and 75%
        ///     <paramref name="endPoint2" />.
        /// </param>
        public static Color InterpolateBetween(Color endPoint1, Color endPoint2, double lambda)
        {
            if (lambda < 0 || lambda > 1)
                throw new ArgumentOutOfRangeException(nameof(lambda));
            return new Color(
                (float) (endPoint1.R * lambda + endPoint2.R * (1 - lambda)),
                (float) (endPoint1.G * lambda + endPoint2.G * (1 - lambda)),
                (float) (endPoint1.B * lambda + endPoint2.B * (1 - lambda)),
                (float) (endPoint1.A * lambda + endPoint2.A * (1 - lambda))
            );
        }

        public static Color FromHex(string hexColor, Color? fallback = null)
        {
            if (hexColor[0] == '#')
            {
                if (hexColor.Length == 9)
                    return new Color(Convert.ToByte(hexColor.Substring(1, 2), 16),
                        Convert.ToByte(hexColor.Substring(3, 2), 16),
                        Convert.ToByte(hexColor.Substring(5, 2), 16),
                        Convert.ToByte(hexColor.Substring(7, 2), 16));
                if (hexColor.Length == 7)
                    return new Color(Convert.ToByte(hexColor.Substring(1, 2), 16),
                        Convert.ToByte(hexColor.Substring(3, 2), 16),
                        Convert.ToByte(hexColor.Substring(5, 2), 16));
                if (hexColor.Length == 5)
                {
                    var r = hexColor[1].ToString();
                    var g = hexColor[2].ToString();
                    var b = hexColor[3].ToString();
                    var a = hexColor[4].ToString();

                    return new Color(Convert.ToByte(r + r, 16),
                        Convert.ToByte(g + g, 16),
                        Convert.ToByte(b + b, 16),
                        Convert.ToByte(a + a, 16));
                }
                if (hexColor.Length == 4)
                {
                    var r = hexColor[1].ToString();
                    var g = hexColor[2].ToString();
                    var b = hexColor[3].ToString();

                    return new Color(Convert.ToByte(r + r, 16),
                        Convert.ToByte(g + g, 16),
                        Convert.ToByte(b + b, 16));
                }
            }

            if (fallback.HasValue)
                return fallback.Value;
            throw new ArgumentException("Invalid color code and no fallback provided.", nameof(hexColor));
        }

        /// <summary>
        ///     Compares whether this Color4 structure is equal to the specified Color4.
        /// </summary>
        /// <param name="other">The Color4 structure to compare to.</param>
        /// <returns>True if both Color4 structures contain the same components; false otherwise.</returns>
        public bool Equals(Color other)
        {
            return
                R == other.R &&
                G == other.G &&
                B == other.B &&
                A == other.A;
        }
    }
}