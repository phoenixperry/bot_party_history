package com.startechplus.unityblebridge;

import java.util.List;
import java.util.UUID;

public class AdvertisedData {
    private List<UUID> mUuids;
    private String mName;
    public AdvertisedData(List<UUID> uuids, String name){
        mUuids = uuids;
        mName = name;
    }

    public List<UUID> getUuids(){
        return mUuids;
    }

    public String getName(){
        return mName;
    }
}
