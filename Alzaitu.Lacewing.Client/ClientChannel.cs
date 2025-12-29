using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Alzaitu.Lacewing.Client
{
	public class ClientChannel : IList<ClientPeer>
	{
		private readonly List<ClientPeer> _joinedClients;

		public bool Close { get; internal set; }
		public bool Hidden { get; internal set; }
		public int Id { get; internal set; }
		public string Name { get; private set; }
		public ClientPeer ChannelMaster { get; set; }

		public ClientChannel(string name)
		{
			_joinedClients = new List<ClientPeer>();
			Name = name;
		}

		public void InitPeerList(int size)
		{
			_joinedClients.Clear();
			_joinedClients.AddRange(new ClientPeer[size]);
		}

		public void ResetPeerList(List<ClientPeer> list)
		{
			_joinedClients.Clear();
			_joinedClients.AddRange(list);
		}

		public static ClientChannel GetChannel(LacewingClient ls, string name)
		{
			if (!ls.globalChannels.ContainsKey(name))
				ls.globalChannels.Add(name, new ClientChannel(name));
			return ls.globalChannels[name];
		}

		public static ClientChannel GetChannelByID(LacewingClient ls, int id)
		{
			if (!ls.globalChannelsByID.TryGetValue(id, out ClientChannel ch))
				return null;
			return ch;
		}

		#region IList

		public IEnumerator<ClientPeer> GetEnumerator() => _joinedClients.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_joinedClients).GetEnumerator();

		public void Add(ClientPeer item) => _joinedClients.Add(item);

		public void Clear() => _joinedClients.Clear();

		public bool Contains(ClientPeer item)
		{
			foreach(ClientPeer peer in _joinedClients)
			{
				if (peer == null)
					continue;
				if (peer.Id == item.Id)
					return true;
			}
			return false;
		}

		public void CopyTo(ClientPeer[] array, int arrayIndex) => _joinedClients.CopyTo(array, arrayIndex);

		public bool Remove(ClientPeer item) => _joinedClients.Remove(item);

		public int Count => _joinedClients.Count;

		public bool IsReadOnly => false;

		public int IndexOf(ClientPeer item) => _joinedClients.IndexOf(item);

		public void Insert(int index, ClientPeer item) => _joinedClients.Insert(index, item);

		public void RemoveAt(int index) => _joinedClients.RemoveAt(index);

		public ClientPeer this[int index]
		{
			get => _joinedClients[index];
			set => _joinedClients[index] = value;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj.GetType() != GetType())
				return false;
			ClientChannel other = (ClientChannel)obj;
			return other.Id == Id
				&& other.Count == Count;
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}
}
