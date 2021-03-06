
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
		/// [GL] Value of GL_REFERENCE_PLANE_SGIX symbol.
		/// </summary>
		[RequiredByFeature("GL_SGIX_reference_plane")]
		public const int REFERENCE_PLANE_SGIX = 0x817D;

		/// <summary>
		/// [GL] Value of GL_REFERENCE_PLANE_EQUATION_SGIX symbol.
		/// </summary>
		[RequiredByFeature("GL_SGIX_reference_plane")]
		public const int REFERENCE_PLANE_EQUATION_SGIX = 0x817E;

		/// <summary>
		/// [GL] Binding for glReferencePlaneSGIX.
		/// </summary>
		/// <param name="equation">
		/// A <see cref="T:double[]"/>.
		/// </param>
		[RequiredByFeature("GL_SGIX_reference_plane")]
		public static void ReferencePlaneSGIX(double[] equation)
		{
			unsafe {
				fixed (double* p_equation = equation)
				{
					Debug.Assert(Delegates.pglReferencePlaneSGIX != null, "pglReferencePlaneSGIX not implemented");
					Delegates.pglReferencePlaneSGIX(p_equation);
					LogCommand("glReferencePlaneSGIX", null, equation					);
				}
			}
			DebugCheckErrors(null);
		}

		internal unsafe static partial class UnsafeNativeMethods
		{
			[SuppressUnmanagedCodeSecurity()]
			[DllImport(Library, EntryPoint = "glReferencePlaneSGIX", ExactSpelling = true)]
			internal extern static unsafe void glReferencePlaneSGIX(double* equation);

		}

		internal unsafe static partial class Delegates
		{
			[RequiredByFeature("GL_SGIX_reference_plane")]
			[SuppressUnmanagedCodeSecurity()]
			internal unsafe delegate void glReferencePlaneSGIX(double* equation);

			[RequiredByFeature("GL_SGIX_reference_plane")]
			[ThreadStatic]
			internal static glReferencePlaneSGIX pglReferencePlaneSGIX;

		}
	}

}
