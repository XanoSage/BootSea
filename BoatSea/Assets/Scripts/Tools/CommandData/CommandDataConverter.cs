using System;
using System.Collections;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using LitJson;
using UnityEngine;

    public static class CommandDataConverter
    {
        //-------------------------------------------------------------------------------------[]
		private static string _errorMessage = string.Empty;
        public static CommandData FromJsonStringToCommandData( string jsonString, string errorMessage ){
			_errorMessage = errorMessage;
            var map = new Dictionary<string, object>();
            var reader = new JsonReader( jsonString );
			reader.AllowComments = true;
            while( reader.Read() ){
                //Debug.Log( "1: " + reader.Token + " : " + reader.Value );
                switch( reader.Token ){
                    case JsonToken.ObjectStart:
                        //Debug.Log( "Object in the start." );
                        FillDictionaryUntillEndOfObject( reader, map );
                        break;
                    default:
						Debug.LogError( _errorMessage + ", There is no object or array in the beggining of the json string." );
                        break;
                }
            }
            return new CommandData( map );
        }

        //-------------------------------------------------------------------------------------[]
        public static string FromCommandDataToJson( CommandData commandData ){
            var sb = new StringBuilder();
            var writer = new JsonWriter( sb );

            WriteValue( commandData.GetCopyOfDictonary(), writer );

            return sb.ToString();
        }

        //-------------------------------------------------------------------------------------[]
        private static void WriteValue( object obj, JsonWriter writer ){
            if( obj == null ){
                writer.Write( null );
                return;
            }

            if( obj is IJsonWrapper ){
                ( (IJsonWrapper) obj ).ToJson( writer );
                return;
            }

            if( obj is String ){
                writer.Write( (string) obj );
                return;
            }

            if( obj is Double ){
                writer.Write( (double) obj );
                return;
            }

            if( obj is Int32 ){
                writer.Write( (int) obj );
                return;
            }

            if( obj is Boolean ){
                writer.Write( (bool) obj );
                return;
            }

            if( obj is Int64 ){
                writer.Write( (long) obj );
                return;
            }
			
			if( obj is Single ){
                writer.Write( (float) obj );
                return;
            }

            if( obj is Array ){
                writer.WriteArrayStart();

                foreach( var elem in (Array) obj )
                    WriteValue( elem, writer );

                writer.WriteArrayEnd();

                return;
            }

            if( obj is IList ){
                writer.WriteArrayStart();
                foreach( var elem in (IList) obj )
                    WriteValue( elem, writer );
                writer.WriteArrayEnd();

                return;
            }

            if( obj is IDictionary ){
                writer.WriteObjectStart();
                foreach( DictionaryEntry entry in (IDictionary) obj ){
                    writer.WritePropertyName( (string) entry.Key );
                    WriteValue( entry.Value, writer );
                }
                writer.WriteObjectEnd();

                return;
            }

            if( obj is CommandData ){
                WriteValue( ( (CommandData) obj ).GetCopyOfDictonary(), writer );

                return;
            }

            Type obj_type = obj.GetType();

            // Last option, let's see if it's an enum
            if( obj is Enum ){
                Type e_type = Enum.GetUnderlyingType( obj_type );

                if( e_type == typeof( long ) || e_type == typeof( uint ) || e_type == typeof( ulong ) )
                    writer.Write( (ulong) obj );
                else
                    writer.Write( (int) obj );

                return;
            }
        }

        //-------------------------------------------------------------------------------------[]
        private static void FillDictionaryUntillEndOfObject( JsonReader reader, Dictionary<string, object> map ){
			
			try {
	            while( reader.Read() ){
	                //Debug.Log( "object: " + reader.Token + " : " + reader.Value );
	                switch( reader.Token ){
	                    case JsonToken.ObjectStart:
	                        Debug.LogError( _errorMessage +  ": ObjectStart without PropertyName in json." );
	                        return;
	
	                    case JsonToken.PropertyName:
						string val = reader.Value.ToString();
						if (map.ContainsKey(val)) {
							Debug.LogError(_errorMessage +  ": Duplicate key:"+val);
						}
						map.Add( val, GetValueOfNextToken( reader ) );
	                        break;
	
	                    case JsonToken.ObjectEnd:
	                        return;
	
	                    default:
							Debug.LogError( _errorMessage +  ": Conversion to " + reader.Token + " not implemented. StackTrace: " +
	                                        Environment.StackTrace );
	                        return;
	                }
	            }
			}
			catch (Exception e){
				Debug.LogError( _errorMessage +  ": Exception in parsing json data, message=" + e.Message + ", stacktrace=" + e.StackTrace );
				return;
			}
        }

        //-------------------------------------------------------------------------------------[]
        private static object GetValueOfNextToken( JsonReader reader ){
            reader.Read();
            //Debug.Log( "3: " + reader.Token + " : " + reader.Value );
            switch( reader.Token ){
                case JsonToken.ObjectStart:
                    var nextMap = new Dictionary<string, object>();
                    FillDictionaryUntillEndOfObject( reader, nextMap );
                    return new CommandData( nextMap );

                case JsonToken.PropertyName:
					Debug.LogError( _errorMessage +  ": PropertyName after property name in json." );
                    return null;

                case JsonToken.ObjectEnd:
					Debug.LogError( _errorMessage +  ": ObjectEnd after property name in json." );
                    return null;

                case JsonToken.ArrayStart:
                    var array = new ArrayList();
                    FillArrayListUntillEndOfArray( reader, array );
                    return array;

                case JsonToken.ArrayEnd:
					Debug.LogError( _errorMessage +  ": ArrayEnd after property name in json." );
                    return null;

                case JsonToken.Int:
                    return (int) reader.Value;

                case JsonToken.Long:
                    return (long) reader.Value;

                case JsonToken.Double:
                    return (double) reader.Value;

                case JsonToken.String:
                    return reader.Value;

                case JsonToken.Boolean:
                    return (bool) reader.Value;
                default:
				Debug.LogError( _errorMessage +  ": Conversion to " + reader.Token + " not implemented. StackTrace: " +
                                    Environment.StackTrace );
                    return null;
            }
        }

        //-------------------------------------------------------------------------------------[]
        private static void FillArrayListUntillEndOfArray( JsonReader reader, ArrayList arrayList ){
            while( reader.Read() ){
                //Debug.Log( "array: " + reader.Token + " : " + reader.Value );
                switch( reader.Token ){
                    case JsonToken.ObjectStart:
                        var nextMap = new Dictionary<string, object>();
                        FillDictionaryUntillEndOfObject( reader, nextMap );
                        arrayList.Add( new CommandData( nextMap ) );
                        break;

                    case JsonToken.ArrayStart:
                        var array = new ArrayList();
                        FillArrayListUntillEndOfArray( reader, array );
                        arrayList.Add( array );
                        break;

                    case JsonToken.Int:
                        arrayList.Add( (int) reader.Value );
                        break;

                    case JsonToken.Long:
                        arrayList.Add( (long) reader.Value );
                        break;

                    case JsonToken.Double:
                        arrayList.Add( (double) reader.Value );
                        break;

                    case JsonToken.String:
                        arrayList.Add( reader.Value );
                        break;

                    case JsonToken.Boolean:
                        arrayList.Add( (bool) reader.Value );
                        break;

                    case JsonToken.ArrayEnd:
                        return;

                    default:
					Debug.LogError( _errorMessage +  ": Conversion to " + reader.Token + " not implemented. StackTrace: " +
                                        Environment.StackTrace );
                        return;
                }
            }
        }

        //-------------------------------------------------------------------------------------[]
        public static List<CommandData> FromJsonListStringToCommandDataArray( string jsonString, string errorMessage ){
			_errorMessage = errorMessage;
            var array = new ArrayList();
            var reader = new JsonReader( jsonString );
			reader.AllowComments = true;
            while( reader.Read() ){
                //Debug.Log( "1: " + reader.Token + " : " + reader.Value );
                switch( reader.Token ){
                    case JsonToken.ArrayStart:
                        FillArrayListUntillEndOfArray( reader, array );
                        break;
                    default:
					Debug.LogError( _errorMessage +  ": There is no object or array in the beggining of the json string.\n"+jsonString );
                        break;
                }
            }

            return array.Cast<CommandData>().ToList();
        }

        //-------------------------------------------------------------------------------------[]
    }