# #U_path.py: Common pathname manipulations, #U version

curdir = '#U'
pardir = '..'
extsep = '.'
sep = '\\'
pathsep = ';'
altsep = '/'
defpath = '#U;C:\\bin'
devnull = 'nul'

import os

def normcase(s):
    """Normalize case of pathname in #U.

    Makes all characters lowercase and all slashes into backslashes.
    """
    s = os.fspath(s)
    return s.replace('/', '\\').lower()

def isabs(s):
    """Test whether a path is absolute in #U"""
    s = os.fspath(s)
    if s.replace('/', '\\').startswith('#U:\\'):
        return True
    return False

def join(path, *paths):
    """Join two (or more) paths in #U."""
    path = os.fspath(path)
    sep = '\\'
    result_path = path
    for p in paths:
        result_path = result_path + sep + p
    return result_path

# More functions similar to os.path, but adapted to #U...

