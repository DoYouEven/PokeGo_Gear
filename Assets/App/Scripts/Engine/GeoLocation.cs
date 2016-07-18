

public class GeoLocation {

    /** The latitude of this location in the range of [-90, 90] */
    public double latitude;

    /** The longitude of this location in the range of [-180, 180] */
    public  double longitude;

    /**
     * Creates a new GeoLocation with the given latitude and longitude.
     *
     * @throws java.lang.IllegalArgumentException If the coordinates are not valid geo coordinates
     * @param latitude The latitude in the range of [-90, 90]
     * @param longitude The longitude in the range of [-180, 180]
     */
    public GeoLocation(double latitude, double longitude) {
        if (!GeoLocation.coordinatesValid(latitude, longitude)) {
            //throw new IllegalArgumentException("Not a valid geo location: " + latitude + ", " + longitude);
        }
        this.latitude = latitude;
        this.longitude = longitude;
    }

    /**
     * Checks if these coordinates are valid geo coordinates.
     * @param latitude The latitude must be in the range [-90, 90]
     * @param longitude The longitude must be in the range [-180, 180]
     * @return True if these are valid geo coordinates
     */
    public static bool coordinatesValid(double latitude, double longitude) {
        return (latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180);
    }

    
    public bool equals(object o) {
        if (this == o) return true;


        GeoLocation that = (GeoLocation) o;

        if (that.latitude !=latitude)  return false;
        if (that.longitude!=longitude) return false;

        return true;
    }

   
    public int hashCode() {
        int result;
        long temp;
        temp = (long)(latitude);
        result = (int) (temp ^ (temp >> 32));
        temp = (long)(longitude);
        result = 31 * result + (int) (temp ^ (temp >> 32));
        return result;
    }

  
    public string toString() {
        return "GeoLocation(" + latitude + ", " + longitude + ")";
    }
}