using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class CommandData
    {
        //===============================================================================================[]
        private readonly Dictionary<string, object> _map = new Dictionary<string, object>();

		public void Clear ()
		{
			_map.Clear();
		}

        //-------------------------------------------------------------------------------------[]
        public CommandData(){
        }

        //-------------------------------------------------------------------------------------[]
        public CommandData( Dictionary<string, object> map ){
            _map = map;
        }

        //-------------------------------------------------------------------------------------[]
	/*
        public CommandData( ServerCommand cmd ){
            _map[MessageField.CommandId] = (int) cmd;
        }

        //-------------------------------------------------------------------------------------[]
        public bool ContainsServerCommand(){
            return _map.ContainsKey( MessageField.CommandId );
        }

        //-------------------------------------------------------------------------------------[]
        public ServerCommand GetServerCommand(){
            return (ServerCommand) GetInt( MessageField.CommandId );
        }
*/
        //-------------------------------------------------------------------------------------[]
        public Dictionary<string, object> GetCopyOfDictonary(){
            return new Dictionary<string, object>( _map );
        }

        //-------------------------------------------------------------------------------------[]
        public object GetRawValue(string key){
            return _map[key];
        }

        //-------------------------------------------------------------------------------------[]
        public int GetInt( string key ){
            CheckKey<int>( key );
            return Convert.ToInt32( _map[key] );
        }

        //-------------------------------------------------------------------------------------[]
        public long GetLong( string key ){
            CheckKey( key );
            return Convert.ToInt64( _map[key] );
        }

        //-------------------------------------------------------------------------------------[]
        public float GetFloat( string key ){
            CheckKey<float>( key );
            return Convert.ToSingle( _map[key] );
        }

        //-------------------------------------------------------------------------------------[]
        public string GetString( string key ){
            CheckKey<string>( key );
            return (string) _map[key];
        }

        //-------------------------------------------------------------------------------------[]
        public bool GetBool( string key ){
            CheckKey<bool>( key );
            return (bool) _map[key];
        }

        //-------------------------------------------------------------------------------------[]
        public CommandData GetCommandData( string key ){
            CheckKey<CommandData>( key );
            return (CommandData) _map[key];
        }

        //-------------------------------------------------------------------------------------[]
        public CommandData GetCommandDataOrNull( string key ){
            if( ContainsKey( key ) )
                return (CommandData) _map[key];
            return null;
        }

        //-------------------------------------------------------------------------------------[]
        public ArrayList GetArray( string key ){
            CheckKey<ArrayList>( key );
            return (ArrayList) _map[key];
        }

        //-------------------------------------------------------------------------------------[]
        public Vector2 GetVector2( string key ){
            CheckKey<CommandData>( key );
            var v = (CommandData) _map[key];
            return new Vector2( v.GetInt( "x" ), v.GetInt( "y" ) );
        }

        //-------------------------------------------------------------------------------------[]
     //   public void SetCommand( ServerCommand cmd ){
      //      _map[MessageField.CommandId] = (int) cmd;
      //  }

        //-------------------------------------------------------------------------------------[]
        public void SetCommandData( string key, CommandData value ){
            _map[key] = value;
        }

        //-------------------------------------------------------------------------------------[]
        public void SetArray( string key, ArrayList value ){
            _map[key] = value;
        }

		//-------------------------------------------------------------------------------------[]
		public void SetVector2( string key, Vector2 value ){
			CommandData vectorData = new CommandData();
			SetCommandData( key, vectorData );
			vectorData.SetInt( "x", (int)value.x );
			vectorData.SetInt( "y", (int)value.y );
		}

        //-------------------------------------------------------------------------------------[]
        public void SetInt( string key, int value ){
            _map[key] = value;
        }

        //-------------------------------------------------------------------------------------[]
        public void SetLong( string key, long value ){
            _map[key] = value;
        }

        //-------------------------------------------------------------------------------------[]
        public void SetFloat( string key, float value ){
            _map[key] = value;
        }

        //-------------------------------------------------------------------------------------[]
        public void SetBool( string key, bool value ){
            _map[key] = value;
        }

        //-------------------------------------------------------------------------------------[]
        public void SetString( string key, string value ){
            _map[key] = value;
        }

		public void SetObject( string key, object value ){
			_map[key] = value;
		}

        //-------------------------------------------------------------------------------------[]
		public bool ContainsKey( string fieldName ){ 
			if (_map==null || fieldName==null) 
				Debug.Log("hi");
            return _map.ContainsKey( fieldName );
        }

        //-------------------------------------------------------------------------------------[]
        public override string ToString(){
			return "[" + GetType() + "]";//+ ToJson();
        }

        //-------------------------------------------------------------------------------------[]
        public string ToJson(){
            return CommandDataConverter.FromCommandDataToJson( this );
        }

        //-------------------------------------------------------------------------------------[]
        private void CheckKey( string key ){
            if( !_map.ContainsKey( key ) )
                throw new Exception( "'" + key + "' key not found in " + typeof( CommandData ) + ", data:\n" + ToJson() );
        }

        //-------------------------------------------------------------------------------------[]
        private void CheckKey<T>( string key ){
            if( !_map.ContainsKey( key ) )
                throw new Exception( "'" + key + "' key not found in " + typeof( CommandData ) + ", data:\n" + ToJson() );
            try{
                var type = (T) Convert.ChangeType( _map[key], typeof( T ) );
            }
            catch( InvalidCastException ){
                throw new Exception( "Requested key '" + key + "' is of type '" + _map[key].GetType() +
                                     "', instead of type '" + typeof( T ) + "'" );
            }
        }

        //===============================================================================================[]
		
		public string[] GetKeys() {
			string[] keys = new string[_map.Keys.Count];
			_map.Keys.CopyTo( keys, 0 );
			return keys;
		}
    }
