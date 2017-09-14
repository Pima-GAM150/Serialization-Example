/*

// Example of XML deserialization method used in Sun and Moon (Legend of the Five Rings collectible card game online client).  Pair with database.xml for reference.

using( XmlReader reader = XmlTextReader.Create( new StringReader(www.text) ) ) {

	while( reader.ReadToFollowing("card") ) {
		Card newCard = new Card();

		reader.MoveToAttribute("id");
		newCard.id = reader.Value;

		reader.MoveToAttribute("type");
		newCard.type = reader.Value;

		while( reader.Read() ) {
			if( reader.NodeType == XmlNodeType.Element ) {
				switch( reader.Name ) {
					case "name":
						newCard.name = reader.ReadElementContentAsString();
					break;

					case "rarity":
						newCard.rarity = reader.ReadElementContentAsString();
					break;

					case "edition":
						string edition = reader.ReadElementContentAsString();
						newCard.edition = edition;
						newCard.editions.Add( edition );
						
						if( reader.Name == "image" ) {
							string imageString = reader.ReadElementContentAsString();
							newCard.images.Add( imageString );
						}

						if( !localDbSets.Contains(edition) ) localDbSets.Add( edition );
					break;

					case "legal":
						newCard.legal.Add( reader.ReadElementContentAsString() );
					break;

					case "text":
						newCard.text = reader.ReadElementContentAsString();
						newCard.text = newCard.text.Replace("<BR>", "<br>");
					break;

					case "artist":
						newCard.artist = reader.ReadElementContentAsString();
					break;

					case "flavor":
						newCard.flavor = reader.ReadElementContentAsString();
					break;

					case "cost":
						int.TryParse( reader.ReadElementContentAsString(), out newCard.cost );
					break;

					case "focus":
						int.TryParse( reader.ReadElementContentAsString(), out newCard.focus );
						if( newCard.focus > -1 ) newCard.deckType = Card.DeckType.Fate;
					break;

					case "clan":
						newCard.clan = reader.ReadElementContentAsString();
					break;

					case "chi":
						newCard.chi = reader.ReadElementContentAsString();
					break;

					case "force":
						newCard.force = reader.ReadElementContentAsString();
					break;

					case "personal_honor":
						string ph = reader.ReadElementContentAsString();
						if( !int.TryParse( ph, out newCard.personal_honor ) ) {
							newCard.personal_honor = -1;
						}
					break;

					case "honor_req":
						string honor_req = reader.ReadElementContentAsString();
						if( honor_req == "-" ) {
							newCard.honor_req = -1;
						}
						else {
							int.TryParse( honor_req, out newCard.honor_req );
						}
					break;

					case "province_strength":
						int.TryParse( reader.ReadElementContentAsString(), out newCard.province_strength );
					break;

					case "gold_production":
						int.TryParse( reader.ReadElementContentAsString(), out newCard.gold_production );
					break;

					case "starting_honor":
						int.TryParse( reader.ReadElementContentAsString(), out newCard.starting_honor );
					break;
				}
			}
			else if( reader.NodeType == XmlNodeType.EndElement ) {
				if( reader.Name == "card" ) {

					// Add to list of all cards and move on to next XML card entry
					TryToAddCard( newCard );
					break;
				}
			}
		}
	}
}

*/