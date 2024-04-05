pragma solidity ^0.8.0;

contract WasteManagement {
    mapping(address => uint) public recyclingPoints;

    event WasteRecycled(address indexed user, uint pointsEarned);

    function recycleWaste(address user, uint weight) public {
        uint points = calculatePoints(weight);
        recyclingPoints[user] += points;
        emit WasteRecycled(user, points);
    }

    function calculatePoints(uint weight) private pure returns (uint) {
        return weight * 10; // Simplified points calculation
    }
}