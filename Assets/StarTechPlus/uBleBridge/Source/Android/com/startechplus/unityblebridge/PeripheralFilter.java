package com.startechplus.unityblebridge;

import java.util.ArrayList;
import java.util.List;

public class PeripheralFilter {
	
	public static final int NONE = 0;
	public static final int PERIPHERAL_UUID = 1;
	public static final int SERVICE_UUID = 2;
	
	private List<String> _filter;
	private int _type;
	
	
	public PeripheralFilter(int type)
	{
		_type = type;
		_filter = new ArrayList<String>();
	}
	
	public void clear()
	{
		_filter.clear();
	}
	
	public void addUuid(String uuid)
	{
		String _uuid = uuid.toUpperCase();
		
		if(!_filter.contains(_uuid))
		{
			_filter.add(_uuid); //add this
		}
	}
	
	public boolean filterWith(String uuid, int uuidType)
	{
		String _uuid = uuid.toUpperCase();
		
		if(_type == NONE || uuidType != _type || _filter.contains(_uuid))
			return true;
		else
			return false;
	}
	
}
