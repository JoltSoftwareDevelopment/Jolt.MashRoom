using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

// todo: change namespace
namespace Ignostic.Studio256.RenderApi
{
    public abstract class AssetManager<T> : IDisposable, IEnumerable<T> where T : IAsset
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        // TODO make _assets private
        public List<T> _assets;
        private Dictionary<string, List<T>> _lookup;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public AssetManager()
        {
            _assets = new List<T>();
            _lookup = new Dictionary<string, List<T>>(StringComparer.InvariantCultureIgnoreCase);

        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public string RootPath
        { 
            get;
            protected set;
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        protected List<T> GetList(string name)
        {
            List<T> resources = null;
            if (!_lookup.TryGetValue(name, out resources))
            {
                resources = new List<T>();
                _lookup.Add(name, resources);
            }
            return resources;
        }

        public void Add(T resource)
        {
            var models = GetList(resource.Name);
            models.Add(resource);
            _assets.Add(resource);
        }

        public abstract T Load(string name);
        //public abstract void LoadAll();
        //public T Clone(string name);

        void IDisposable.Dispose()
        {
            _lookup.Clear();
            foreach (var item in _assets)
            {
                item.Dispose();
            }
            _assets.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _assets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _assets.GetEnumerator();
        }

        public T this[string name]
        {
            get
            {
                return Load(name);
            }
        }

    }
}