//
//  iOSNative.mm
//  Unity-iPhone
//
//  Created by dev on 2014. 11. 25..
//
//

// Returns a char* (a string to Unity)
extern "C"
{
    char* CStringToString(const char* text)
    {
        if (text == NULL)
            return NULL;
        
        char* res = (char*)malloc(strlen(text) + 1);
        strcpy(res, text);
		
        return res;
    }
    
    NSString* CstringToNsString(const char* text)
    {
        if (text != NULL)
            return [NSString stringWithUTF8String:text];
        else
            return [NSString stringWithUTF8String:""];
    }
    
    //---
    int _pow2(int x)
    {
        return x * x;
    }
    
    char* _helloWorldString()
    {
        NSString *helloString = @"Hello World";
        // UTF8String method gets us a c string. Then we have to malloc a copy to give to Unity. I reuse a method below that makes it easy.
        return CStringToString([helloString UTF8String]);
    }
    
    char* _combineStrings(const char* text1, const char* text2)
    {
        NSString* nsText1 = CstringToNsString(text1);
        NSString* nsText2 = CstringToNsString(text2);
        NSString* combinedString = [NSString stringWithFormat:@"%@ %@", nsText1, nsText2];
        return CStringToString([combinedString UTF8String]);
    }
    
    char* _convertNfdToNfc(const char* text)
    {
        NSString* nsText1 = CstringToNsString(text);
		NSLog ([nsText1 precomposedStringWithCompatibilityMapping]);
		NSLog ([nsText1 precomposedStringWithCanonicalMapping]);
		NSLog ([nsText1 decomposedStringWithCompatibilityMapping]);
		NSLog ([nsText1 decomposedStringWithCanonicalMapping]);

        NSString* nsCanonical = [nsText1 precomposedStringWithCanonicalMapping];
        return CStringToString([nsCanonical UTF8String]);
    }
}