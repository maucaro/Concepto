REPLACE=$(printf '%s\n' "$1" | sed -e 's/[\/&]/\\&/g')
sed -i "0,/^\([[:space:]]*[[:space:]\-] image: *\).*/s//\1$REPLACE/;" $2
echo "Contents of " $2 
echo "-------"
cat $2