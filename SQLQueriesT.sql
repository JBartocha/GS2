/*
DELETE from Food;
DELETE from FoodSettings;
DELETE from GameNumbers;
DELETE from SnakeMoves;
*/

/*
SELECT * FROM GameNumbers
LEFT JOIN FoodSettings FS 
ON GameNumbers.GameNumbers_ID = FS.GameID
LEFT JOIN Food FO
ON FS.FoodID = FO.Food_ID;
*/

/*
SELECT GameNumbers.GameNumbers_ID, GameNumbers.Date, SM.MoveNumber, SM.Direction, FoodFromMoves.PosX, FoodFromMoves.PosY FROM GameNumbers
LEFT JOIN SnakeMoves SM
ON GameNumbers.GameNumbers_ID = SM.GameID
LEFT JOIN Food FoodFromMoves
ON SM.FoodID = FoodFromMoves.Food_ID
WHERE GameNumbers_ID = 17;
*/

SELECT * FROM GameNumbers
LEFT JOIN SnakeMoves SM
ON GameNumbers.GameNumbers_ID = SM.GameID
LEFT JOIN Food FoodFromMoves
ON SM.FoodID = FoodFromMoves.Food_ID
WHERE GameNumbers_ID = 17;