# GS2 

# Popis

**Testovací projekt**

Hra had. Obsahuje následující prvky:
- Ovládání klávesnicí (tlačítka **AWSD**), pomocí Myši nebo obě možnosti.
Při ovládání přes myš bere poslední pozici myši při pohybu na herní ploše a podle ní nastavuje směr).
Pomocí tlačítka nebo tlačítka 'p' lze hru pozastavit


Sekce s nastavením "Options", která umožňuje nastavení:

    - Nastavení velikosti bloků
    - Nastavení množství řádků a sloupců herní plochy
    - Nastavení ovládání přes myš, klávesnici nebo obojí
    - Množství jídla na herní ploše
    - Počáteční rychlost
    - Množství jídla
    - Množství jídla potřebné na zvýšení obtížnosti
    - Procentuální navýšení rychlosti při zvýšení obtížnosti

V sekci nastavení je "Nastav zdi" tlačítko, které umožňuje individuálně nastavit kde budou generované zdi. Pokud se v hlavním nastavovacím formuláři neuloží nastavení tak i nastavení v "Nastav Zdi" nebude uloženo.


Hra také umožňuje po ukončení hry (prohrou) uložit do lokální MS SQL databáze uložit záznam hry, který může být pomocí tlačítka "Load Replay" a následného stlačení tlačítka "Start" přehrán od začátku do konce.

Při stlačení tlačítka "Restart" je simulace/hra zrušena a nachystána nová hra(pozastavená).

### Třída Settings

Hlavní nastavovací prkvy.
Veškerá 'globální nastavení' jsou uložena v třídě Settings, která je při změnách uložena do .json souboru jako výchozí nastavení pro příští zapnutí.

Modifikovatelné v OptionsForm (formulář pro nastavení):
```
JsonSaveFileName
UseMousePositionToMove 
UseKeyboardToMove 
FoodCount 
LevelIncreaseInterval
TickInMilliseconds
DifficultyIncrease
CellSize
Rows
Columns
WallPositions
```
Tyto proměnné se mění v průběhu hry:
```
Level
FoodsEaten
Moves
HeadPosition
GameOver
ForbiddenDirection
SnakeStartingHeadPosition
Pause
CurrentSpeed
```

### Poznánky

Využití obojího .json a databáze je podle mě logicky nesmyslné ale chtěl jsem použít oba způsoby pro procvičení.
WFA pro tento projekt není nejlepší hlavně kvůli vykreslování, které při překlesování způsobuje blikání. To se objevuje i přes to, že bylo výrazně zmírněno skrze regionální vykreslování jen v oblastech kde je tomu potřeba.

### TODO

- Přidat nastavení možnosti přecházet s hadem z jednoho okraje na opačný (tento stav by se zároveň nepovažoval jako konec hry).
- Ošetřit stav kdy je celá herní plocha zaplněná a není co kam přidat.
- Upravit zhled hada tak, aby bylo možné kdykolik poznat kudy trasoval cestu (něco jako přidat čáry na vykreslované body hada).
- Opravit blikání při přidávání bloků v WallOptionsForm.
- Ve WallOptionsForm využít třídu "Grid".
