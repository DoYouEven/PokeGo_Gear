
# encoding: utf-8

# Root Pokemon Image Link
from bs4 import BeautifulSoup 

import json

# JSON structure



import re
non_decimal = re.compile(r'[^\d.]+')




rootLink = ""


# Structure of the data 
# No 
# Name
# Type ?
# Hp
# Attack
# Defense
# Evolution Requirements
# Fast Attacks
# Specials Attacks

import urllib

def createPokemon(id,name,types,hp,attack,defense,reqCandy,egg,attacks,specialAttacks):
        self.dict["id"] = id
        self.dict["name"] = name
        self.dict["attack"] = attack
        self.dict["types"] = types
        self.dict["defense"] = defense
        self.dict["reqCandy"] = reqCandy
        self.dict["egg"] = egg
        self.dict["attacks"]= attacks
        self.dict["specialAttacks"] = specialAttacks

def download(imageLink): 
    filename = imageLink.split('/')[-1]
    f = open('{0}'.format(filename),'wb')
    f.write(urllib.urlopen('http://www.serebii.net/{0}'.format(imageLink)).read())
    f.close()

def main():
    fh = open("pokemon.html")
    soup = BeautifulSoup(fh,"html.parser")

    # trArr = soup.findAll('td',{"class":"fooinfo"})
    trArr = soup.findAll('tr')
    pokemon = []
    for i,j in enumerate(trArr):
        
        
    

        if i == 0 or i == 1 or i == 2 or i == 3 or i == 4 or i == 5 or i == len(trArr)-1:
            print "Data Not Useful"
            pass
        elif i % 2 == 0:
            poke = {}
            poke["fastAttacks"] = []
            poke["type"] = []
            print "Data Start"
            #print j

            typesNumberImage =  j.findAll('td',{"class":"cen"})

            for u,v in enumerate(typesNumberImage):
                #print "{0} {1}".format(u,v)

                if u == 0:
                    poke["number"] = v.text.replace(" ","").replace("#","").replace("\n","").encode('utf-8')
                elif u == 1:
                    img = v.findAll("img")[0]
                    poke["imageLink"] = img["src"]
                    download(poke["imageLink"])

                elif u == 2:
                    types = v.findAll("img")

                    for t in types:
                        
                        pkmts = t["src"]
                        words = pkmts.split("/")
                        pkmt = words[-1]
                        pkmt = pkmt.replace('.gif',"")
                        poke["type"].append(pkmt)



            info =  j.findAll("td",{"class":"fooinfo"})
            
            for u,v in enumerate(info):
                #print "{0} {1} Info ".format(u,v)
                pokeData = v.text.replace(" ","").replace("\n","").encode("utf-8")

                print info
                
                if u == 0:
                    poke["name"] = pokeData
                    print "name {0}".format(pokeData)
                elif u == 1:
                    poke["hp"] = pokeData
                    print "hp {0}".format(pokeData)
                elif u == 2:
                    poke["attack"] = pokeData
                    print "attack {0}".format(pokeData)
                elif u == 3:
                    poke["defense"] = pokeData
                    print "defense {0}".format(pokeData)
                elif u == 4:
                    poke["candy"] = non_decimal.sub('', pokeData.decode("utf-8"))
                    print "Candy {0}".format(pokeData)
                elif u == 5:
                    poke["egg"] = pokeData
                    print "Egg {0}".format(pokeData)
                elif u == 6:
                    attacks = v.findAll('a')
                    for attack in attacks:
                        atk = attack.text
                        poke["fastAttacks"].append(atk)

            pokemon.append(poke)
            print "Data End"
        else:
            print "Image Start"
            print j
            print "Image End"
        
        
    jsonObj = json.dumps(pokemon,indent=4, sort_keys=True)
    with open('pokemon.json','w') as fh:
        json.dump(jsonObj,fh)

def downloadImages(imageLink):
    pass

if __name__ == '__main__':
    main()