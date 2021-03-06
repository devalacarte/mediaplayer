// ArtistSearch.cs
//
//  Copyright (C) 2008 Amr Hassan
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using System.Xml;
using System.Collections.Generic;

namespace Lastfm.Services
{
	/// <summary>
	/// Encapsulates the artist searching functions.
	/// </summary>
	public class ArtistSearch : Search<Artist>
	{	
		
		public ArtistSearch(string name, Session session)
			:base("artist", session)
		{
			this.searchTerms["artist"] = name;
		}
		
		/// <summary>
		/// Returns a page of results.
		/// </summary>
		/// <param name="page">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="Artist"/>
		/// </returns>
		public override Artist[] GetPage(int page)
		{
			if (page < 1)
				throw new InvalidPageException(page, 1);
			
			RequestParameters p = getParams();
			p["page"] = page.ToString();
			
			XmlDocument doc = request(prefix + ".search", p);
			
			List<Artist> list = new List<Artist>();			
			foreach(XmlNode n in doc.GetElementsByTagName("artist"))
				list.Add(new Artist(extract(n, "name"), Session));
			
			return list.ToArray();
		}
	}
}
