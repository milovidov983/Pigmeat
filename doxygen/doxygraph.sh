git clone https://github.com/jitsuCM/doxygraph.git
cd doxygraph-master
sudo apt install flex -y
sudo apt install Perl -y
sudo apt install automake -y
sudo apt install bison -y
sudo apt install libtool -y
sudo apt install byacc -y
sudo apt install cpanminus -y
sudo apt install python-pydot python-pydot-ng graphviz -y
sudo cpanm lib::abs
sudo cpanm namespace::autoclean
sudo cpanm Moose
sudo cpanm XML::Rabbit::Root
sudo cpanm Hash::FieldHash
sudo cpanm GraphViz2
perl doxygraph/doxygraph/doxygraph ./OUT/xml/index.xml doxygraph/doxyviz/htdocs/graph.dot
cp -avr ./doxygraph/doxyviz/htdocs ./OUT/html/diagrams