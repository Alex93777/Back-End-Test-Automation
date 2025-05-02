import { expect } from "chai";
import { isSymmetric } from '../2.CheckForSymmetry.js';

describe("isSymmetric function tests", function(){
    it("should return true with symmetric array input", function() {
        //arrange
        let inputData = [1, 2, 3, 2, 1]
        let result;

        //act
        result = isSymmetric(inputData)

        //assert
        expect(result).to.equal(true)
    })

    it("should return true with empthy array", function(){
        //arrange
        let inputData = []
        let result;

        //act
        result = isSymmetric(inputData)

        //assert
        expect(result).to.equal(true)
    })

    it("should return false for non symmetric", function(){
        //arrange
        let inputData = [1, 2, 3, 5, 1]
        let result;

        //act
        result = isSymmetric(inputData)

        //assert
        expect(result).to.equal(false)
    })

    it("should return false with non-array", function(){
        //act
        let result = isSymmetric("this is not array");

        //assert
        expect(result).to.equal(false);
    })

    it("should return false for non-number elements", function(){
        //arrange
        let inputData = [1, '1'];
        let result;

        //act
        result = isSymmetric(inputData);

        //assert
        expect(result).to.equal(false);
    })

    it("should return true for single element array", function(){
        //act
        let result = isSymmetric([1]);

        //assert
        expect(result).to.equal(true);
    })
})