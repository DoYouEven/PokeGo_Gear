

using System;
public class GeoHash
{
    private string geoHash;

    // The default precision of a geohash
    private  const int DEFAULT_PRECISION = 10;

    // The maximal precision of a geohash
    public  const int MAX_PRECISION = 22;

    // The maximal number of bits precision for a geohash
   // public const int MAX_PRECISION_BITS = MAX_PRECISION * Base32Utils.BITS_PER_BASE32_CHAR;

    public GeoHash(double latitude, double longitude)
    {
        createGeoHash(latitude, longitude, DEFAULT_PRECISION);
    }

    public GeoHash(GeoLocation location)
    {
        createGeoHash(location.latitude, location.longitude, DEFAULT_PRECISION);
    }
    void createGeoHash(double latitude, double longitude, int precision)
    {
        if (precision < 1)
        {
            //throw new IllegalArgumentException("Precision of GeoHash must be larger than zero!");
        }
        if (precision > MAX_PRECISION)
        {
            // throw new IllegalArgumentException("Precision of a GeoHash must be less than " + (MAX_PRECISION + 1) + "!");
        }
        if (!GeoLocation.coordinatesValid(latitude, longitude))
        {
            //throw new IllegalArgumentException(String.format("Not valid location coordinates: [%f, %f]", latitude, longitude));
        }
        double[] longitudeRange = { -180, 180 };
        double[] latitudeRange = { -90, 90 };

        char[] buffer = new char[precision];

        for (int i = 0; i < precision; i++)
        {
            int hashValue = 0;
            for (int j = 0; j < Base32Utils.BITS_PER_BASE32_CHAR; j++)
            {
                bool even = (((i * Base32Utils.BITS_PER_BASE32_CHAR) + j) % 2) == 0;
                double val = even ? longitude : latitude;
                double[] range = even ? longitudeRange : latitudeRange;
                double mid = (range[0] + range[1]) / 2;
                if (val > mid)
                {
                    hashValue = (hashValue << 1) + 1;
                    range[0] = mid;
                }
                else
                {
                    hashValue = (hashValue << 1);
                    range[1] = mid;
                }
            }
            buffer[i] = Base32Utils.valueToBase32Char(hashValue);
        }
        this.geoHash = new string(buffer);
    }

    public GeoHash(double latitude, double longitude, int precision)
    {
        createGeoHash(latitude, longitude, precision);
    }

    public GeoHash(string hash)
    {
        int i = 5;

        if (hash.Length == 0 || !Base32Utils.isValidBase32String(hash))
        {
            throw new ArgumentException("Not a valid geoHash: " + hash);
        }
        this.geoHash = hash;
    }

    public String getGeoHashString()
    {
        return this.geoHash;
    }


    public bool equals(Object o)
    {
        if (this == o) return true;
        if (o == null) return false;

        GeoHash other = (GeoHash)o;

        return this.geoHash.Equals(other.geoHash);
    }


    public String toString()
    {
        return "GeoHash{" +
                "geoHash='" + geoHash + '\'' +
                '}';
    }


    public int hashCode()
    {
        return this.geoHash.GetHashCode();
    }
}