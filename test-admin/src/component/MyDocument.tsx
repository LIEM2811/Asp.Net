import React from "react";
import {
  Image,
  Text,
  View,
  Page,
  Document,
  StyleSheet,
} from "@react-pdf/renderer";
import logo from "./img/LogoHITC.png";

const MyDocument = ({ data }) => {
  const { cartId, products } = data;

  // Check if products exist, or initialize an empty array
  const validProducts = Array.isArray(products) ? products : [];

  // Calculate total price based on products array
  const calculatedTotalPrice = validProducts.reduce((acc, product) => {
    return acc + product.price * product.quantity;
  }, 0);

  const styles = StyleSheet.create({
    page: {
      fontSize: 11,
      paddingTop: 20,
      paddingLeft: 40,
      paddingRight: 40,
      lineHeight: 1.5,
      flexDirection: "column",
    },
    spaceBetween: {
      flexDirection: "row",
      justifyContent: "space-between",
      alignItems: "center",
      color: "#3E3E3E",
    },
    titleContainer: {
      flexDirection: "row",
      marginTop: 24,
    },
    logo: {
      width: 300,
    },
    addressTitle: {
      fontSize: 15,
      fontWeight: "bold",
    },
    address: {
      fontSize: 13,
    },
    theader: {
      fontSize: 10,
      fontWeight: "bold",
      padding: 4,
      backgroundColor: "#DEDEDE",
      borderColor: "#E0E0E0",
      borderWidth: 1,
    },
    tbody: {
      fontSize: 9,
      padding: 4,
      borderColor: "#E0E0E0",
      borderWidth: 1,
    },
    total: {
      fontSize: 10,
      padding: 4,
      fontWeight: "bold",
      borderColor: "#E0E0E0",
      borderWidth: 1,
    },
  });

  const InvoiceTitle = () => (
    <View style={styles.titleContainer}>
      <Image style={styles.logo} src={logo} />
    </View>
  );

  const UserAddress = () => (
    <View style={{ marginBottom: 20 }}>
      <Text style={styles.addressTitle}>
        Email: <Text style={styles.address}>{localStorage.getItem("username")}</Text>
      </Text>
      <Text style={styles.addressTitle}>
        Total Price: <Text style={styles.address}>{calculatedTotalPrice.toFixed(2)}</Text>
      </Text>
    </View>
  );

  const TableHead = () => (
    <View style={{ flexDirection: "row", marginTop: 10 }}>
      <View style={[styles.theader, { flex: 2 }]}>
        <Text>Items</Text>
      </View>
      <View style={[styles.theader, { flex: 1 }]}>
        <Text>Price</Text>
      </View>
      <View style={[styles.theader, { flex: 1 }]}>
        <Text>Qty</Text>
      </View>
      <View style={[styles.theader, { flex: 1 }]}>
        <Text>Amount</Text>
      </View>
    </View>
  );

  const TableBody = () =>
    validProducts.map((product) => (
      <View
        style={{ flexDirection: "row" }}
        key={product.productId || product.productName}
      >
        <View style={[styles.tbody, { flex: 2 }]}>
          <Text>{product.productName}</Text>
        </View>
        <View style={[styles.tbody, { flex: 1 }]}>
          <Text>{product.price.toFixed(2)}</Text>
        </View>
        <View style={[styles.tbody, { flex: 1 }]}>
          <Text>{product.quantity}</Text>
        </View>
        <View style={[styles.tbody, { flex: 1 }]}>
          <Text>{(product.price * product.quantity).toFixed(2)}</Text>
        </View>
      </View>
    ));

  const TableTotal = () => (
    <View style={{ flexDirection: "row" }}>
      <View style={[styles.total, { flex: 2 }]}>
        <Text></Text>
      </View>
      <View style={[styles.total, { flex: 1 }]}>
        <Text></Text>
      </View>
      <View style={[styles.total, { flex: 1 }]}>
        <Text>Total</Text>
      </View>
      <View style={[styles.total, { flex: 1 }]}>
        <Text>{calculatedTotalPrice.toFixed(2)}</Text>
      </View>
    </View>
  );

  return (
    <Document>
      <Page size="A4" style={styles.page}>
        <InvoiceTitle />
        <UserAddress />
        <TableHead />
        {validProducts.length > 0 ? (
          <>
            <TableBody />
            <TableTotal />
          </>
        ) : (
          <Text>No products available</Text>
        )}
      </Page>
    </Document>
  );
};

export default MyDocument;
