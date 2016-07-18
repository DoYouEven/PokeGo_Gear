using System;
using com.firebase.geofire.core.GeoHash;
using com.google.firebase.database.DatabaseError;
using com.google.firebase.database.DatabaseReference;
using com.google.firebase.database.DataSnapshot;
using com.google.firebase.database.GenericTypeIndicator;
using com.google.firebase.database.ValueEventListener;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;
/**
 * A GeoFire instance is used to store geo location data in Firebase.
 */
namespace com.firebase.geofire {
public class GeoFire {

    /**
     * A listener that can be used to be notified about a successful write or an error on writing.
     */
    public  interface CompletionListener {
        /**
         * Called once a location was successfully saved on the server or an error occurred. On success, the parameter
         * error will be null; in case of an error, the error will be passed to this method.
         *
         * @param key   The key whose location was saved
         * @param error The error or null if no error occurred
         */
        public void onComplete(string key, DatabaseError error);
    }

    /**
     * A small wrapper class to forward any events to the LocationEventListener.
     */
    private  class LocationValueEventListener implements ValueEventListener {

        private readonly LocationCallback callback;

        LocationValueEventListener(LocationCallback callback) {
            this.callback = callback;
        }

        
        public override void onDataChange(DataSnapshot dataSnapshot) {
            if (dataSnapshot.getValue() == null) {
                this.callback.onLocationResult(dataSnapshot.getKey(), null);
            } else {
                GeoLocation location = GeoFire.getLocationValue(dataSnapshot);
                if (location != null) {
                    this.callback.onLocationResult(dataSnapshot.getKey(), location);
                } else {
                    string message = "GeoFire data has invalid format: " + dataSnapshot.getValue();
                    this.callback.onCancelled(DatabaseError.fromException(new Exception(message)));
                }
            }
        }

        
        public override void onCancelled(DatabaseError databaseError) {
            this.callback.onCancelled(databaseError);
        }
    }

    static GeoLocation getLocationValue(DataSnapshot dataSnapshot) {
        try {
            GenericTypeIndicator<IDictionary<string, object>> typeIndicator = new GenericTypeIndicator<IDictionary<string, object>>() {};
            IDictionary<string, object> data = dataSnapshot.getValue(typeIndicator);
            IList<?> location = (IList<?>) data.get("l");
            Number latitudeObj = (Number) location.get(0);
            Number longitudeObj = (Number) location.get(1);
            double latitude = latitudeObj.doubleValue();
            double longitude = longitudeObj.doubleValue();
            if (location.size() == 2 && GeoLocation.coordinatesValid(latitude, longitude)) {
                return new GeoLocation(latitude, longitude);
            } else {
                return null;
            }
        } catch (NullPointerException e) {
            return null;
        } catch (ClassCastException e) {
            return null;
        }
    }

    private readonly DatabaseReference databaseReference;
    private readonly EventRaiser eventRaiser;

    /**
     * Creates a new GeoFire instance at the given Firebase reference.
     *
     * @param databaseReference The Firebase reference this GeoFire instance uses
     */
    public GeoFire(DatabaseReference databaseReference) {
        this.databaseReference = databaseReference;
        EventRaiser eventRaiser;
        try {
            eventRaiser = new AndroidEventRaiser();
        } catch (Exception e) {
            // We're not on Android, use the ThreadEventRaiser
            eventRaiser = new ThreadEventRaiser();
        }
        this.eventRaiser = eventRaiser;
    }

    /**
     * @return The Firebase reference this GeoFire instance uses
     */
    public DatabaseReference getDatabaseReference() {
        return this.databaseReference;
    }

    DatabaseReference getDatabaseRefForKey(string key) {
        return this.databaseReference.child(key);
    }

    /**
     * Sets the location for a given key.
     *
     * @param key      The key to save the location for
     * @param location The location of this key
     */
    public void setLocation(string key, GeoLocation location) {
        this.setLocation(key, location, null);
    }

    /**
     * Sets the location for a given key.
     *
     * @param key                The key to save the location for
     * @param location           The location of this key
     * @param completionListener A listener that is called once the location was successfully saved on the server or an
     *                           error occurred
     */
    public void setLocation( string key,  GeoLocation location,  CompletionListener completionListener) {
        if (key == null) {
            throw new NullPointerException();
        }
        DatabaseReference keyRef = this.getDatabaseRefForKey(key);
        GeoHash geoHash = new GeoHash(location);
        IDictionary<string, object> updates = new Hashtable<string, object>();
        updates.put("g", geoHash.getGeoHashString());
        updates.put("l", (location.latitude, location.longitude));
        if (completionListener != null) {
            keyRef.setValue(updates, geoHash.getGeoHashString(), new DatabaseReference.CompletionListener() {
                
                public override void onComplete(DatabaseError databaseError, DatabaseReference databaseReference) {
                    completionListener.onComplete(key, databaseError);
                }
            });
        } else {
            keyRef.setValue(updates, geoHash.getGeoHashString());
        }
    }

    /**
     * Removes the location for a key from this GeoFire.
     *
     * @param key The key to remove from this GeoFire
     */
    public void removeLocation(string key) {
        this.removeLocation(key, null);
    }

    /**
     * Removes the location for a key from this GeoFire.
     *
     * @param key                The key to remove from this GeoFire
     * @param completionListener A completion listener that is called once the location is successfully removed
     *                           from the server or an error occurred
     */
    public void removeLocation( string key,  CompletionListener completionListener) {
        if (key == null) {
            throw new NullPointerException();
        }
        DatabaseReference keyRef = this.getDatabaseRefForKey(key);
        if (completionListener != null) {
            keyRef.setValue(null, new DatabaseReference.CompletionListener() {
                
                public override void onComplete(DatabaseError databaseError, DatabaseReference databaseReference) {
                    completionListener.onComplete(key, databaseError);
                }
            });
        } else {
            keyRef.setValue(null);
        }
    }

    /**
     * Gets the current location for a key and calls the callback with the current value.
     *
     * @param key      The key whose location to get
     * @param callback The callback that is called once the location is retrieved
     */
    public void getLocation(string key, LocationCallback callback) {
        DatabaseReference keyRef = this.getDatabaseRefForKey(key);
        LocationValueEventListener valueListener = new LocationValueEventListener(callback);
        keyRef.addListenerForSingleValueEvent(valueListener);
    }

    /**
     * Returns a new Query object centered at the given location and with the given radius.
     *
     * @param center The center of the query
     * @param radius The radius of the query, in kilometers
     * @return The new GeoQuery object
     */
    public GeoQuery queryAtLocation(GeoLocation center, double radius) {
        return new GeoQuery(this, center, radius);
    }

    void raiseEvent(Runnable r) {
        this.eventRaiser.raiseEvent(r);
    }
}
}