
// Copyright (C) 2015-2017 Luca Piccioni
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

#pragma warning disable 649, 1572, 1573

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace OpenGL
{
	public partial class Gl
	{
		/// <summary>
		/// [GL] Value of GL_RGBA_FLOAT32_ATI symbol.
		/// </summary>
		[RequiredByFeature("GL_ATI_texture_float")]
		public const int RGBA_FLOAT32_ATI = 0x8814;

		/// <summary>
		/// [GL] Value of GL_RGB_FLOAT32_ATI symbol.
		/// </summary>
		[RequiredByFeature("GL_ATI_texture_float")]
		public const int RGB_FLOAT32_ATI = 0x8815;

		/// <summary>
		/// [GL] Value of GL_ALPHA_FLOAT32_ATI symbol.
		/// </summary>
		[RequiredByFeature("GL_ATI_texture_float")]
		public const int ALPHA_FLOAT32_ATI = 0x8816;

		/// <summary>
		/// [GL] Value of GL_INTENSITY_FLOAT32_ATI symbol.
		/// </summary>
		[RequiredByFeature("GL_ATI_texture_float")]
		public const int INTENSITY_FLOAT32_ATI = 0x8817;

		/// <summary>
		/// [GL] Value of GL_LUMINANCE_FLOAT32_ATI symbol.
		/// </summary>
		[RequiredByFeature("GL_ATI_texture_float")]
		public const int LUMINANCE_FLOAT32_ATI = 0x8818;

		/// <summary>
		/// [GL] Value of GL_LUMINANCE_ALPHA_FLOAT32_ATI symbol.
		/// </summary>
		[RequiredByFeature("GL_ATI_texture_float")]
		public const int LUMINANCE_ALPHA_FLOAT32_ATI = 0x8819;

		/// <summary>
		/// [GL] Value of GL_RGBA_FLOAT16_ATI symbol.
		/// </summary>
		[RequiredByFeature("GL_ATI_texture_float")]
		public const int RGBA_FLOAT16_ATI = 0x881A;

		/// <summary>
		/// [GL] Value of GL_RGB_FLOAT16_ATI symbol.
		/// </summary>
		[RequiredByFeature("GL_ATI_texture_float")]
		public const int RGB_FLOAT16_ATI = 0x881B;

		/// <summary>
		/// [GL] Value of GL_ALPHA_FLOAT16_ATI symbol.
		/// </summary>
		[RequiredByFeature("GL_ATI_texture_float")]
		public const int ALPHA_FLOAT16_ATI = 0x881C;

		/// <summary>
		/// [GL] Value of GL_INTENSITY_FLOAT16_ATI symbol.
		/// </summary>
		[RequiredByFeature("GL_ATI_texture_float")]
		public const int INTENSITY_FLOAT16_ATI = 0x881D;

		/// <summary>
		/// [GL] Value of GL_LUMINANCE_FLOAT16_ATI symbol.
		/// </summary>
		[RequiredByFeature("GL_ATI_texture_float")]
		public const int LUMINANCE_FLOAT16_ATI = 0x881E;

		/// <summary>
		/// [GL] Value of GL_LUMINANCE_ALPHA_FLOAT16_ATI symbol.
		/// </summary>
		[RequiredByFeature("GL_ATI_texture_float")]
		public const int LUMINANCE_ALPHA_FLOAT16_ATI = 0x881F;

	}

}
