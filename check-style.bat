@echo off
echo Checking WackyRaces code style compliance...

set "errorCount=0"

echo.
echo Checking for file-scoped namespaces...
for /r %%f in (*.cs) do (
    echo Checking: %%f | find "bin\" >nul 2>&1 || (
        findstr /r "namespace.*{" "%%f" >nul 2>&1 && (
            echo ERROR: %%f - Uses block-scoped namespace instead of file-scoped
            set /a errorCount+=1
        )
    )
)

echo.
echo Checking for sealed test classes...
for /r %%f in (*Test*.cs) do (
    echo Checking: %%f | find "bin\" >nul 2>&1 || (
        findstr /c:"[TestClass]" "%%f" >nul 2>&1 && (
            findstr /c:"public class " "%%f" >nul 2>&1 && (
                echo ERROR: %%f - Test class should be sealed
                set /a errorCount+=1
            )
        )
    )
)

echo.
echo ========================
if %errorCount% equ 0 (
    echo SUCCESS: All files follow WackyRaces code style!
    exit /b 0
) else (
    echo ERRORS FOUND: %errorCount%
    echo Please fix the issues above
    exit /b 1
)
