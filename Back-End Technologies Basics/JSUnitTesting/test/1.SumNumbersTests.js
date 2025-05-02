import { expect } from "chai";
import { sum } from '../1.SumNumbers.js';

describe("Sum function tests", function() {
    it("should return the sum of array of numbers", function() {
        //arrange
        let testData = [1, 2, 3]
        let result;
        
        //act
        result = sum(testData);

        //assert
        expect(result).to.equal(6)
    })

    it("should return the sum of array of strings", function() {
        //arrange
        let testData = ["1", "2", "3"]
        let result;
        
        //act
        result = sum(testData);

        //assert
        expect(result).to.equal(6)
    })

    it("should return 0 when pass array with 0 elements", function() {
        //arrange
        let testData = []
        let result;
        
        //act
        result = sum(testData);

        //assert
        expect(result).to.equal(0)
    })

    it("should return correct sum when pass negative numbers", function() {
        //arrange
        let testData = [-1, -5, -4]
        let result;
        
        //act
        result = sum(testData);

        //assert
        expect(result).to.equal(-10)
    })

    it("should return correct sum when pass mixed input", function() {
        //arrange
        let testData = ["1", 2, "-1"]
        let result;
        
        //act
        result = sum(testData);

        //assert
        expect(result).to.equal(2)
    })

    it("should return correct sum when chars as input", function() {
        //arrange
        let testData = ['a', 'b', 'c']
        let result;
        
        //act
        result = sum(testData);

        //assert
        expect(result).to.NaN
    })
})